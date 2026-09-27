using AutoMapper;
using c__2026_data;
using c__nRepository_2026.Entities;
using c__nRepository_2026.Interfaces;
using c__nRepository_2026.Repositories;
using c__service_2026.Interfaces;
using c__service_2026.Profiles;
using c__service_2026.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=FaceGalleryDb;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<IdentificationContext>(
    options => options.UseSqlServer(connectionString)
);

builder.Services.AddScoped<IContext>(
    sp => sp.GetRequiredService<IdentificationContext>()
);

// Repositories

builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<IDetectedRepository, DetectedCharacterRepository>();
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// AutoMapper

builder.Services.AddAutoMapper(typeof(MappingProfile));

// Services

builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IDetectedService, DetectedCharacterService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IUserService, UserService>();

// Azure Vision

builder.Services.AddScoped<IAzureVisionService>(_ =>
    new AzureVisionService(
        builder.Configuration["Azure:VisionEndpoint"] ?? string.Empty,
        builder.Configuration["Azure:VisionApiKey"] ?? string.Empty
    )
);

// Image Recognition

bool faceRecognitionEnabled =
    builder.Configuration.GetValue<bool>("FaceRecognition:Enabled");

if (faceRecognitionEnabled)
{
    builder.Services.AddSingleton<
        IFaceMatchingService,
        FaceAiSharpMatchingService
    >();
}
else
{
    var modelPath = Path.Combine(
        builder.Environment.ContentRootPath,
        "Models",
        "yolov8n.onnx"
    );

    builder.Services.AddSingleton<IFaceMatchingService>(
        _ => new YoloObjectMatchingService(modelPath)
    );
}

builder.Services.AddScoped<IRecognitionService>(sp =>
    new RecognitionService(
        sp.GetRequiredService<IImageRepository>(),
        sp.GetRequiredService<IDetectedRepository>(),
        sp.GetRequiredService<IFaceMatchingService>(),
        builder.Environment.WebRootPath
    )
);

// Controllers

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

var app = builder.Build();

// Database initialization

using (var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider
            .GetRequiredService<IdentificationContext>();

    context.Database.EnsureCreated();

    // No demo galleries or demo images.

    if (!context.Users.Any())
    {
        context.Users.Add(new User
        {
            Username = "local-user",
            Email = "local@facegallery.local",
            Password = "local"
        });
    }

    if (!context.Characters.Any())
    {
        context.Characters.Add(new Character
        {
            CharacterName = "Unknown",
            Description = "Default character",
            CreatedDate = DateTime.Now
        });
    }

    context.SaveChanges();
}

// Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
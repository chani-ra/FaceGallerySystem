# Setup

## Prerequisites

- Visual Studio 2022 (17.8+) with the **ASP.NET and web development**
  workload, which includes .NET 8 SDK and SQL Server LocalDB.
- No internet connection is required at run time. Visual Studio will need
  internet the first time it restores NuGet packages (normal for any
  project).

## First run

1. Open `FaceGallerySystem.sln` in Visual Studio.
2. Wait for NuGet package restore to finish (bottom status bar).
3. Right-click **FaceGallery.Api** in Solution Explorer → **Set as Startup
   Project**.
4. Press **F5** (or Ctrl+F5 to run without the debugger attached).
5. A browser opens `http://localhost:5080/index.html`.

On first run, the app automatically:
- Creates the `FaceGalleryDb` database on your local SQL Server LocalDB
  instance (no migrations needed).
- Seeds one demo user and one default "Unknown" character, which the demo
  UI uses behind the scenes so you don't need a login screen for the demo.

## If LocalDB isn't available

Some machines don't have SQL Server LocalDB installed (it's part of the
"Data storage and processing" component of the ASP.NET workload). You have
two options:

**Option A - install LocalDB** (recommended, no code changes):
In Visual Studio Installer → Modify → make sure "SQL Server Express
LocalDB" is checked under Individual Components, then re-run.

**Option B - switch to SQLite** (fully local file, zero install):
1. In `FaceGallery.Data/FaceGallery.Data.csproj`, replace the
   `Microsoft.EntityFrameworkCore.SqlServer` package reference with
   `Microsoft.EntityFrameworkCore.Sqlite` (same version number).
2. In `FaceGallery.Api/Program.cs`, change `options.UseSqlServer(...)` to
   `options.UseSqlite(connectionString)`.
3. In `appsettings.json`, change the connection string to:
   `"Data Source=facegallery.db"`.

## First-run download (one-time, needs internet)

The real face-recognition engine (`FaceAiSharpMatchingService`) ships as a
NuGet package (`FaceAiSharp.Bundle`) that includes its ONNX models, so the
**only** internet access needed is the normal one-time NuGet restore when
you first open the solution - nothing is downloaded at app run time, and
nothing leaves your machine once it's running (all inference is local).

The very first time you run a search, loading those models into memory can
take a few seconds - the demo page shows a note about this. Subsequent
searches are fast.

## Optional: Azure AI Vision (person/face detection)

The **recognition** feature (the core "find this person in the gallery"
flow) works fully offline via a real local face-recognition model (see
`docs/ARCHITECTURE.md`) and needs no Azure key - skip this section for the
demo.

Azure AI Vision is only used by the *separate*, optional
`/api` processing path already present in the original service layer,
which detects that a person exists in a photo (bounding boxes), not who
they are. If you want to try it:

1. Create a free-tier (**F0**) Azure AI Vision resource in the Azure
   Portal - F0 includes 5,000 free calls/month, no payment needed for a
   demo.
2. Copy the endpoint and key into `appsettings.json` under `Azure:VisionEndpoint`
   and `Azure:VisionApiKey`.

Note: Azure's `AnalyzeAsync(Uri, ...)` call (used by `AzureVisionService`)
needs the image to be reachable at a public URL, which a `localhost` URL is
not. This path is therefore mainly useful once the app is deployed
somewhere public - for local development, the built-in local recognition
pipeline (see `docs/ARCHITECTURE.md`) is what to demo.

## Troubleshooting

| Symptom | Likely cause / fix |
|---|---|
| "Cannot open database... LocalDB" | Install LocalDB (see above) or switch to SQLite. |
| NuGet restore fails for `SixLabors.ImageSharp`, `FaceAiSharp.Bundle`, or `Microsoft.ML.OnnxRuntime` version | Right-click the solution → Manage NuGet Packages for Solution → update the package on `FaceGallery.Service` to its latest available version. |
| First search takes a few seconds / times out | Expected on the very first call - the face-detection/recognition ONNX models are loading into memory. Try again, it will be fast afterwards. |
| "No face was detected" on every demo-data image | Expected for the synthetic demo images - see `docs/DEMO_DATA.md`. Upload 2-3 real photos instead for a reliable live demo. |
| Port 5080 already in use | Edit `FaceGallery.Api/Properties/launchSettings.json` and change `applicationUrl`. |
| Uploaded images don't show up | Make sure you selected the gallery you uploaded to in the dropdown, then click "רענון" (refresh). |

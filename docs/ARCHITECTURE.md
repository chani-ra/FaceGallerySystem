# Architecture

## Project layout

```
FaceGallerySystem.sln
src/
  FaceGallery.Repository/   Entities + generic Repository<T> pattern (data access)
  FaceGallery.Data/         EF Core DbContext (IdentificationContext)
  FaceGallery.Service/      DTOs, business-logic Services, AutoMapper profile,
                             Azure integration, the recognition engine(s), demo-data seeder
  FaceGallery.Api/          ASP.NET Core Web API host: Controllers, DI wiring,
                             Swagger, static demo page (wwwroot/index.html)
```

This is a classic layered (n-tier) architecture:

```
Api  ->  Service  ->  Repository  <-  Data
```

`Api` depends on all three lower layers to wire everything together with
dependency injection. `Service` only depends on `Repository`'s
*interfaces* (`IGalleryRepository`, `IImageRepository`, ...), never on EF
Core directly - that's what lets you unit-test the service layer with a
fake repository, and swap the database technology (see `docs/SETUP.md`,
SQLite option) without touching business logic.

## Data model

```
User 1---* Gallery 1---* Image 1---* DetectedCharacter *---1 Character
```

- **User** - who owns a gallery (single demo user is seeded automatically;
  the login/register logic in `UserService` is already implemented and
  ready to wire up to a real login screen later).
- **Character** - a named person/character you're tracking across photos.
- **Gallery** - a named collection of photos, associated with one
  character and one user.
- **Image** - one uploaded photo, belonging to a gallery.
- **DetectedCharacter** - one AI detection result for one image: a
  confidence score and a bounding box (`FaceCoordinates`, `"x,y,w,h"`).

## The recognition pipeline (the core feature)

This is what answers "which photos show this person?" - either within one
gallery, or across every gallery one user owns.

```
reference photo ──┐
                   ├─► IFaceMatchingService.ComputeDescriptorAsync ──► descriptor A
gallery image 1 ───┤                                                        \
gallery image 2 ───┤                                                         Compare (similarity)
gallery image N ───┘                                                        /
                        IFaceMatchingService.ComputeDescriptorAsync ──► descriptor B
```

Orchestrated by `RecognitionService` (`IRecognitionService`), which has two
entry points sharing one internal comparison loop:
- `FindMatchesAsync(photo, galleryId)` - search one gallery.
- `FindMatchesForUserAsync(photo, userId)` - search every image the user owns,
  across all their galleries (`/api/recognize/by-user`), for when you don't
  want to pick a gallery first.

Both: compute a descriptor for the uploaded reference photo, compute one for
every candidate image (skipping images where no face could be found - see
below), rank by similarity, and flag anything above the threshold
(`IFaceMatchingService.SuggestedMatchThreshold`, unless the caller overrides
it) as a match.

### `IFaceMatchingService` - the swappable matching engine

This interface is the seam that lets the matching technology change without
touching `RecognitionService`, the controllers, or the DTOs. A descriptor is
an **opaque object** - each implementation decides its own concrete shape and
only needs to understand its own descriptors in `Compare()`.

**Default: `FaceAiSharpMatchingService` - real face recognition.**
Uses the [FaceAiSharp](https://github.com/georg-jung/FaceAiSharp) library
(MIT-licensed), which runs two pretrained ONNX models locally via ONNX
Runtime, both bundled by the `FaceAiSharp.Bundle` NuGet package (no separate
model download step):
1. **SCRFD** detects the face and its landmarks (eyes, nose, mouth corners).
2. The face is aligned/cropped using those landmarks.
3. **ArcFace** turns the aligned face into a 512-dimensional embedding - a
   vector that's close to other embeddings of the *same person* and far from
   embeddings of *different people*, regardless of background, lighting, or
   angle.

Comparison is the cosine similarity (dot product of two unit vectors) between
two embeddings. The FaceAiSharp project's own documented guidance:
`dot >= 0.42` → same person, `dot <= 0.28` → different people, in between →
ambiguous. `SuggestedMatchThreshold` is set to `0.42` accordingly.

If no face can be detected in a photo, `ComputeDescriptorAsync` throws
`NoFaceDetectedException`. For the reference photo, that's a `400 Bad
Request` back to the caller (nothing to compare against). For one photo
*inside* a gallery being searched, `RecognitionService` catches it and marks
just that one result with a `note`, instead of failing the whole search.

**Fallback: `ImageSimilarityService` - local color-layout descriptor.**
No face detection needed - it works on any image, including the
procedurally generated demo images (see `docs/DEMO_DATA.md`). It resizes
the image to a 12x12 grid and uses the average color of each cell as a
432-number vector, compared by cosine similarity. This is a legitimate,
classic content-based-image-retrieval (CBIR) technique, not a random
number - but it compares overall visual appearance (colour/composition),
not facial geometry, so it's a much weaker signal than real face
recognition. It's kept in the codebase specifically to demonstrate that the
interface really is swappable - and to browse/demo the UI without needing
real face photos on hand. To switch back to it, change one line in
`Program.cs`:
```csharp
builder.Services.AddSingleton<IFaceMatchingService, ImageSimilarityService>();
```
Nothing else in the app needs to change.

## Demo data

See `docs/DEMO_DATA.md` for what gets auto-generated on first run and its
one honest limitation (synthetic images vs. a real face-detection model).

## Why SQL Server LocalDB + `EnsureCreated()` instead of migrations

For a project that needs to "just run" on someone else's machine with a
single F5, `Database.EnsureCreated()` avoids requiring `dotnet ef database
update` as a manual step. The trade-off: it doesn't support incrementally
evolving the schema the way EF Core Migrations do. If you keep working on
this after the interview, switching to Migrations
(`Add-Migration InitialCreate` in Package Manager Console) is a natural
next step - the DbContext is already structured to support it.

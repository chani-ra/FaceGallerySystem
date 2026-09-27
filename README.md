# Face Gallery System

Upload a reference photo of one person, and the system searches a photo
gallery (or everything you own, across galleries) and returns every image
that person appears in - ranked by similarity, using real face detection +
face recognition (ArcFace embeddings), running fully locally.

This is a .NET 8 / ASP.NET Core Web API with a small built-in web page for
demoing the flow end-to-end, built on the layered architecture
(Repository / Data / Service / Api) already designed for this project.

## Quick start (Visual Studio)

1. Open `FaceGallerySystem.sln`.
2. Set **FaceGallery.Api** as the startup project (right-click it → *Set as
   Startup Project*).
3. Press **F5**. A browser tab opens the demo page automatically.
4. Try it instantly with the auto-generated demo data (top of the page), or
   create a gallery, upload a few real photos, then upload one reference
   photo and click "זהי" (Identify).

No database setup, no `dotnet ef` commands, and no paid cloud service are
required - see [`docs/SETUP.md`](docs/SETUP.md) if step 3 doesn't "just
work" on your machine.

## Documentation

| File | What's in it |
|---|---|
| [`docs/SETUP.md`](docs/SETUP.md) | Prerequisites, first run, troubleshooting, optional Azure setup |
| [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) | Project layout, data model, how the recognition pipeline works |
| [`docs/API.md`](docs/API.md) | Every HTTP endpoint, with example requests |
| [`docs/DEMO_DATA.md`](docs/DEMO_DATA.md) | What the auto-seeded demo galleries are, and their one honest limitation |
| [`docs/INTERVIEW_NOTES.md`](docs/INTERVIEW_NOTES.md) | Honest talking points for explaining the design choices |

## What's in this solution

The original solution (data / repository / service layers) already had a
well-designed schema and mostly-complete CRUD services - see
`docs/ARCHITECTURE.md` for what was reused as-is. Added on top:

- A single buildable solution with a real ASP.NET Core Web API host
  (`FaceGallery.Api`) - Controllers, Swagger, static file hosting, zero-setup
  local SQL Server LocalDB.
- **Real face recognition**: `IFaceMatchingService` → `FaceAiSharpMatchingService`,
  using SCRFD face detection + ArcFace embeddings, fully local. Search a
  single gallery (`/api/recognize`) or everything a user owns across all
  their galleries (`/api/recognize/by-user`).
- A simpler local color-similarity engine (`ImageSimilarityService`) kept in
  the codebase as a working example of how the matching engine can be
  swapped behind the same interface.
- Auto-generated demo data (3 galleries, synthetic images, reference
  photos) so the app can be explored without uploading anything first.
- A minimal demo web page (`wwwroot/index.html`).

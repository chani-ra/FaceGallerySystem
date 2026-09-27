# Talking points

Honest notes on what's solid, what's a deliberate trade-off, and what
you'd say if asked "why didn't you do X?". Good interviewers respond much
better to "here's the trade-off I made and why" than to an unexamined claim
that everything is perfect.

## What's genuinely solid here

- **Layered architecture** with clean separation: Entities/Repositories
  don't know about DTOs; Services don't know about EF Core or ASP.NET;
  Controllers are thin. This is why the recognition feature, the by-user
  search, and the real AI model could each be added as isolated changes
  without touching the existing CRUD code.
- **Generic repository pattern** (`IRepository<T>`) with specific
  interfaces layered on top for per-entity queries.
- **DTO/AutoMapper separation** from the EF entities.
- **A real Strategy pattern in production, not just in theory**:
  `IFaceMatchingService` has two working implementations
  (`FaceAiSharpMatchingService` for real face recognition,
  `ImageSimilarityService` as a simpler fallback), and swapping between
  them is a one-line change in `Program.cs` - nothing else in the app
  knows or cares which one is active. This is a genuinely good thing to
  point at and explain if asked "how would you swap this out later?".

## How the real recognition works (say this cold)

Two ONNX models, both running locally via ONNX Runtime, both bundled by the
`FaceAiSharp.Bundle` NuGet package (no manual model download step):
1. **SCRFD** detects the face and its landmarks.
2. **ArcFace** converts the aligned face into a 512-number embedding vector.

Two photos of the *same person* produce embeddings with a high cosine
similarity (close to 1); different people produce a low or negative one.
The library's own documented thresholds (`>= 0.42` same person, `<= 0.28`
different) are used directly rather than invented ones.

## The one honest simplification: the demo data

The 3 auto-seeded demo galleries use procedurally-drawn colored circles,
not real face photos - see `docs/DEMO_DATA.md`. They're there to show the
pipeline (upload → store → rank → UI) without needing photos on hand, but a
real face-detection model may not find a "face" in a drawn circle, and the
UI will honestly say so (`note: "No face was detected..."`) rather than
faking a result. For a live demo of actual recognition accuracy, use 2-3
real photos of one person instead - that's what the model is built for, and
it's a 10-second upload.

If asked directly: *"The demo data proves the pipeline end-to-end without
needing photos on hand. For the actual recognition accuracy I'll show it on
a couple of real photos, since that's what a face-detection model is
trained on."*

## Other things worth knowing cold

- Why SQL Server LocalDB + `EnsureCreated()` instead of Migrations: zero
  setup for whoever runs it, trade-off is no incremental schema
  versioning (see `docs/ARCHITECTURE.md`).
- Why Azure AI Vision is optional and not the recognition engine: Azure's
  People-detection feature only answers "is there a person in this photo",
  not "which person" - and it needs a publicly reachable image URL, which a
  local upload isn't. The real "which person" question needed a local,
  offline, always-on comparison instead.
- Descriptors are recomputed on every search rather than cached on upload -
  simple and correct, fine for a demo-sized gallery; caching them
  (e.g. storing the embedding alongside the `Image` row) is the natural
  next optimization for a larger gallery.
- What you'd do with another week: cache embeddings on upload instead of
  recomputing per search; add authentication instead of the single seeded
  demo user; add EF Core Migrations; expose the already-implemented
  `CharacterService` search/statistics methods as endpoints.

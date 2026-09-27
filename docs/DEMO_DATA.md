# Demo data

On first run (only if the database has no galleries yet), the app
automatically creates:

- **3 characters / 3 galleries** - "דמות כתומה", "דמות כחולה", "דמות ירוקה"
  (orange/blue/green), each with 5 images: 3 in that character's color and
  2 "distractor" images borrowed from the other two characters' colors.
- **1 separate reference photo per character**, available via
  `GET /api/demo/references` and shown at the top of the demo page with a
  "השתמשי בזו" (use this one) button, so you can click through the whole
  flow - pick a reference, search, see ranked results - without uploading
  anything yourself.

This is implemented in `FaceGallery.Service/Services/DemoDataSeeder.cs`,
purely with per-pixel drawing (no image files shipped, no external
download): each "face" is a colored circle with two dot eyes and a mouth,
drawn directly with `SixLabors.ImageSharp`.

## The one honest catch

These are simple procedural graphics, **not photographs of real faces**.
The active recognition engine (`FaceAiSharpMatchingService`) is a real
face-*detection* model trained on photographs - it may or may not find a
"face" in a drawn circle with two dots. If it finds none, that image comes
back in the results with `note: "No face was detected in this image."`
instead of crashing the whole search - but it means the demo galleries are
best used to show the **pipeline** (upload → store → rank → UI), not
necessarily a convincing live accuracy demo.

**For a live demo that reliably shows real face recognition working**,
upload 2-3 real photos of one person (yourself, with permission) through
section 2 of the page, then upload one more photo of that same person as
the reference in section 3. That will go through real face detection +
ArcFace embedding + cosine similarity end to end, and is what to actually
run in front of the interviewer.

If asked about this directly, this is a good, honest answer: *"the demo
data proves the pipeline works end-to-end without needing photos on hand;
for the actual face-recognition accuracy I'll show it on a couple of real
photos, since that's what the model is built for."*

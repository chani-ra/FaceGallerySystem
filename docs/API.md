# API Reference

Base URL when running locally: `http://localhost:5080`. Full interactive
docs are also available at `/swagger` while the app is running in
Development mode.

## Galleries

### `GET /api/galleries`
Returns every gallery (with image count).

### `GET /api/galleries/{id}`
Returns one gallery, including its list of images.

### `POST /api/galleries`
```json
{ "name": "אלבום חתונה" }
```
Creates a gallery (under the seeded demo user/character). Returns the
created gallery.

### `DELETE /api/galleries/{id}`
Deletes a gallery.

## Images

### `GET /api/galleries/{galleryId}/images`
Lists every image in a gallery.

### `POST /api/galleries/{galleryId}/images`
`multipart/form-data` with a `file` field (jpg/jpeg/png/webp/bmp, up to
15 MB). Stores the photo and returns its DB record (id + URL).

### `DELETE /api/images/{id}`
Deletes one image.

## Recognition (the core feature)

### `POST /api/recognize?galleryId={id}&threshold={optional}`
Search a single gallery. `multipart/form-data` with a `file` field: the
reference photo of the person to search for.

### `POST /api/recognize/by-user?userId={id}&threshold={optional}`
Same thing, but searches every image across every gallery that user owns,
instead of picking one gallery first.

Both return every candidate image ranked by similarity:
```json
[
  { "imageId": 12, "galleryId": 3, "url": "/uploads/3/ab12.jpg", "scorePercent": 91.4, "isMatch": true, "note": null },
  { "imageId": 9,  "galleryId": 3, "url": "/uploads/3/cc90.jpg", "scorePercent": 12.0, "isMatch": false, "note": null },
  { "imageId": 15, "galleryId": 3, "url": "/uploads/3/de44.jpg", "scorePercent": 0,    "isMatch": false, "note": "No face was detected in this image." }
]
```
`threshold` is optional - omit it to use the active recognition engine's
own recommended threshold (`0.42` for the default real face-recognition
engine). `note` is set (and `scorePercent` is `0`) when that specific photo
couldn't be compared - most commonly because no face was detected in it.

If **no face is found in the reference photo itself**, the endpoint returns
`400 Bad Request` with a message explaining why, since there's nothing to
compare against.

### `GET /api/demo/references`
Lists the auto-generated demo reference photos (see `docs/DEMO_DATA.md`),
for the "try it without uploading a photo" flow in the UI.
```json
[{ "characterId": 2, "characterName": "דמות כתומה - הדגמה", "url": "/demo-references/character-2.png" }]
```

## Characters

### `GET /api/characters`, `GET /api/characters/{id}`, `POST /api/characters`
Basic CRUD over the `Character` entity, reusing the existing
`CharacterService` (search-by-name and statistics methods are already
implemented there too, just not yet exposed as endpoints - trivial to add
if you want them for the demo).

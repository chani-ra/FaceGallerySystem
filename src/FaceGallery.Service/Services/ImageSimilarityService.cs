using c__service_2026.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace c__service_2026.Services
{
    /// <summary>
    /// Local color-layout descriptor - a classic, lightweight content-based
    /// image retrieval (CBIR) technique. Unlike FaceAiSharpMatchingService,
    /// this implementation does NOT require a detectable face, so it also
    /// works on the procedurally generated demo images (see
    /// docs/DEMO_DATA.md) and on non-photographic images in general.
    ///
    /// It is not the default recognition engine (see Program.cs) - it's kept
    /// as a documented, working example of how easily IFaceMatchingService
    /// can be swapped, and as a no-real-face-needed fallback for exploring
    /// the app's UI/API without uploading real photos.
    /// </summary>
    public class ImageSimilarityService : IFaceMatchingService
    {
        private const int GridSize = 12;

        public double SuggestedMatchThreshold => 0.75;

        public async Task<object> ComputeDescriptorAsync(Stream imageStream, string? faceCoordinates = null)
        {
            using var image = await Image.LoadAsync<Rgba32>(imageStream);

            if (!string.IsNullOrWhiteSpace(faceCoordinates))
            {
                var crop = TryParseCropRectangle(faceCoordinates, image.Width, image.Height);
                if (crop.HasValue)
                {
                    image.Mutate(ctx => ctx.Crop(crop.Value));
                }
            }

            image.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(GridSize, GridSize),
                Mode = ResizeMode.Stretch
            }));

            var vector = new float[GridSize * GridSize * 3];
            int idx = 0;
            for (int y = 0; y < GridSize; y++)
            {
                for (int x = 0; x < GridSize; x++)
                {
                    Rgba32 pixel = image[x, y];
                    vector[idx++] = pixel.R / 255f;
                    vector[idx++] = pixel.G / 255f;
                    vector[idx++] = pixel.B / 255f;
                }
            }

            Normalize(vector);
            return vector;
        }

        public double Compare(object descriptorA, object descriptorB)
        {
            var a = (float[])descriptorA;
            var b = (float[])descriptorB;
            if (a.Length != b.Length)
                throw new ArgumentException("Descriptors must have the same length to be compared.");

            double dot = 0;
            for (int i = 0; i < a.Length; i++) dot += a[i] * b[i];

            if (dot < 0) dot = 0;
            if (dot > 1) dot = 1;
            return dot;
        }

        private static void Normalize(float[] vector)
        {
            double sumSquares = 0;
            foreach (var v in vector) sumSquares += v * v;
            double norm = Math.Sqrt(sumSquares);
            if (norm < 1e-8) return;
            for (int i = 0; i < vector.Length; i++) vector[i] = (float)(vector[i] / norm);
        }

        private static Rectangle? TryParseCropRectangle(string faceCoordinates, int imageWidth, int imageHeight)
        {
            var parts = faceCoordinates.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length != 4) return null;

            if (!int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int x) ||
                !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out int y) ||
                !int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out int w) ||
                !int.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out int h))
            {
                return null;
            }

            x = Math.Clamp(x, 0, Math.Max(0, imageWidth - 1));
            y = Math.Clamp(y, 0, Math.Max(0, imageHeight - 1));
            w = Math.Clamp(w, 1, imageWidth - x);
            h = Math.Clamp(h, 1, imageHeight - y);

            if (w <= 0 || h <= 0) return null;
            return new Rectangle(x, y, w, h);
        }
    }
}

using FaceAiSharp;
using c__service_2026.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace c__service_2026.Services
{
    public class FaceAiSharpMatchingService : IFaceMatchingService
    {
        private readonly IFaceDetectorWithLandmarks _detector;
        private readonly IFaceEmbeddingsGenerator _recognizer;

        private readonly object _processingLock = new();

        public double SuggestedMatchThreshold => 0.42;

        public FaceAiSharpMatchingService()
        {
            _detector =
                FaceAiSharpBundleFactory
                    .CreateFaceDetectorWithLandmarks();

            _recognizer =
                FaceAiSharpBundleFactory
                    .CreateFaceEmbeddingsGenerator();
        }

        public async Task<object> ComputeDescriptorAsync(
            Stream imageStream,
            string? faceCoordinates = null)
        {
            if (imageStream == null || !imageStream.CanRead)
            {
                throw new ArgumentException(
                    "The image stream must be readable.",
                    nameof(imageStream)
                );
            }

            using var image =
                await Image.LoadAsync<Rgb24>(imageStream);

            lock (_processingLock)
            {
                var faces = _detector.DetectFaces(image);

                if (!faces.Any())
                {
                    throw new NoFaceDetectedException(
                        "לא זוהו פנים בתמונה."
                    );
                }

                var bestFace = faces
                    .OrderByDescending(face => face.Confidence)
                    .First();

                if (bestFace.Landmarks == null)
                {
                    throw new NoFaceDetectedException(
                        "זוהו פנים, אך לא ניתן היה לזהות את נקודות הפנים."
                    );
                }

                _recognizer.AlignFaceUsingLandmarks(
                    image,
                    bestFace.Landmarks
                );

                float[] embedding =
                    _recognizer.GenerateEmbedding(image);

                return embedding;
            }
        }

        public double Compare(
            object descriptorA,
            object descriptorB)
        {
            if (descriptorA is not float[] a ||
                descriptorB is not float[] b)
            {
                throw new ArgumentException(
                    "Invalid face descriptors."
                );
            }

            if (a.Length != b.Length)
            {
                throw new ArgumentException(
                    "Face descriptors have different lengths."
                );
            }

            double dot = 0;

            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
            }

            return Math.Clamp(dot, 0, 1);
        }
    }
}
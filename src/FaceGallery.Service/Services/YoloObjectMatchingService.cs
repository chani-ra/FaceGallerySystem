using c__service_2026.Interfaces;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace c__service_2026.Services
{
    public sealed class YoloObjectMatchingService : IFaceMatchingService
    {
        private const int DefaultSize = 640;
        private const int ClassCount = 80;

        private const float MinDetectionConfidence = 0.25f;

        private readonly string _modelPath;

        private readonly Lazy<InferenceSession> _session;

        public static readonly string[] ClassNames =
        {
            "person", "bicycle", "car", "motorcycle",
            "airplane", "bus", "train", "truck",
            "boat", "traffic light", "fire hydrant",
            "stop sign", "parking meter", "bench",
            "bird", "cat", "dog", "horse",
            "sheep", "cow", "elephant", "bear",
            "zebra", "giraffe", "backpack",
            "umbrella", "handbag", "tie",
            "suitcase", "frisbee", "skis",
            "snowboard", "sports ball", "kite",
            "baseball bat", "baseball glove",
            "skateboard", "surfboard",
            "tennis racket", "bottle",
            "wine glass", "cup", "fork",
            "knife", "spoon", "bowl",
            "banana", "apple", "sandwich",
            "orange", "broccoli", "carrot",
            "hot dog", "pizza", "donut",
            "cake", "chair", "couch",
            "potted plant", "bed", "dining table",
            "toilet", "tv", "laptop",
            "mouse", "remote", "keyboard",
            "cell phone", "microwave",
            "oven", "toaster", "sink",
            "refrigerator", "book", "clock",
            "vase", "scissors", "teddy bear",
            "hair drier", "toothbrush"
        };

        public sealed class Descriptor
        {
            internal Descriptor(float[] scores)
            {
                Scores = scores;
            }

            public float[] Scores { get; }

            public int PrimaryClassId
            {
                get
                {
                    int best = -1;

                    float confidence = MinDetectionConfidence;

                    for (int i = 0; i < Scores.Length; i++)
                    {
                        if (Scores[i] > confidence)
                        {
                            confidence = Scores[i];
                            best = i;
                        }
                    }

                    return best;
                }
            }

            public float PrimaryConfidence =>
                PrimaryClassId >= 0
                    ? Scores[PrimaryClassId]
                    : 0f;

            public string PrimaryLabel =>
                PrimaryClassId >= 0
                    ? ClassNames[PrimaryClassId]
                    : "none";
        }

        public double SuggestedMatchThreshold => 0.40;

        public YoloObjectMatchingService(string modelPath)
        {
            _modelPath =
                modelPath
                ?? throw new ArgumentNullException(
                    nameof(modelPath)
                );

            _session = new Lazy<InferenceSession>(
                LoadSession,
                LazyThreadSafetyMode.ExecutionAndPublication
            );
        }

        private InferenceSession LoadSession()
        {
            if (!File.Exists(_modelPath))
            {
                throw new FileNotFoundException(
                    "The YOLO model is missing. " +
                    "Put yolov8n.onnx in FaceGallery.Api/Models.",
                    _modelPath
                );
            }

            return new InferenceSession(
                _modelPath,
                new SessionOptions
                {
                    GraphOptimizationLevel =
                        GraphOptimizationLevel.ORT_ENABLE_ALL
                }
            );
        }

        public async Task<object> ComputeDescriptorAsync(
            Stream imageStream,
            string? faceCoordinates = null
        )
        {
            if (imageStream == null || !imageStream.CanRead)
            {
                throw new ArgumentException(
                    "Image stream must be readable.",
                    nameof(imageStream)
                );
            }

            var session = _session.Value;

            var input = session.InputMetadata.First();

            var dimensions = input.Value.Dimensions;

            int height =
                dimensions.Length == 4 && dimensions[2] > 0
                    ? dimensions[2]
                    : DefaultSize;

            int width =
                dimensions.Length == 4 && dimensions[3] > 0
                    ? dimensions[3]
                    : DefaultSize;

            if (dimensions.Length != 4 || dimensions[1] != 3)
            {
                throw new InvalidDataException(
                    "YOLO model must use NCHW input."
                );
            }

            using var image =
                await Image.LoadAsync<Rgb24>(imageStream);

            image.Mutate(ctx =>
                ctx.AutoOrient().Resize(
                    new ResizeOptions
                    {
                        Size = new Size(width, height),

                        Mode = ResizeMode.Pad,

                        Position =
                            AnchorPositionMode.Center,

                        PadColor =
                            Color.FromRgb(114, 114, 114)
                    }
                )
            );

            var tensor = new DenseTensor<float>(
                new[] { 1, 3, height, width }
            );

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixel = image[x, y];

                    tensor[0, 0, y, x] = pixel.R / 255f;
                    tensor[0, 1, y, x] = pixel.G / 255f;
                    tensor[0, 2, y, x] = pixel.B / 255f;
                }
            }

            using var results = session.Run(
                new[]
                {
                    NamedOnnxValue.CreateFromTensor(
                        input.Key,
                        tensor
                    )
                }
            );

            var output =
                results.First().AsTensor<float>();

            var shape =
                output.Dimensions.ToArray();

            if (
                shape.Length != 3 ||
                shape[0] != 1 ||
                !(shape[1] == 84 || shape[2] == 84)
            )
            {
                throw new InvalidDataException(
                    "Expected standard YOLOv8 detection output."
                );
            }

            bool channelFirst = shape[1] == 84;

            int boxes =
                channelFirst
                    ? shape[2]
                    : shape[1];

            var maxScores =
                new float[ClassCount];

            for (int box = 0; box < boxes; box++)
            {
                float bestConfidence =
                    MinDetectionConfidence;

                int bestClass = -1;

                for (
                    int classId = 0;
                    classId < ClassCount;
                    classId++
                )
                {
                    float confidence =
                        channelFirst
                            ? output[0, classId + 4, box]
                            : output[0, box, classId + 4];

                    if (confidence > bestConfidence)
                    {
                        bestConfidence = confidence;
                        bestClass = classId;
                    }
                }

                if (
                    bestClass >= 0 &&
                    bestConfidence > maxScores[bestClass]
                )
                {
                    maxScores[bestClass] =
                        bestConfidence;
                }
            }

            return new Descriptor(maxScores);
        }

        public double Compare(
            object descriptorA,
            object descriptorB
        )
        {
            var reference =
                descriptorA as Descriptor
                ?? throw new ArgumentException(
                    "Invalid reference descriptor.",
                    nameof(descriptorA)
                );

            var candidate =
                descriptorB as Descriptor
                ?? throw new ArgumentException(
                    "Invalid candidate descriptor.",
                    nameof(descriptorB)
                );

            int targetClass =
                reference.PrimaryClassId;

            if (targetClass < 0)
            {
                return 0;
            }

            return candidate.Scores[targetClass];
        }
    }
}
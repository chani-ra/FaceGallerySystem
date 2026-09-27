using c__nRepository_2026.Entities;
using c__nRepository_2026.Interfaces;
using c__service_2026.Dto;
using c__service_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace c__service_2026.Services
{
    public class RecognitionService : IRecognitionService
    {
        private readonly IImageRepository _imageRepository;

        private readonly IDetectedRepository _detectionRepository;

        private readonly IFaceMatchingService _engine;

        private readonly string _webRootPath;

        public RecognitionService(
            IImageRepository imageRepository,
            IDetectedRepository detectionRepository,
            IFaceMatchingService engine,
            string webRootPath
        )
        {
            _imageRepository = imageRepository;
            _detectionRepository = detectionRepository;
            _engine = engine;
            _webRootPath = webRootPath;
        }

        public async Task<List<MatchResultDto>> FindMatchesAsync(
            Stream referencePhoto,
            int galleryId,
            double? matchThreshold = null
        )
        {
            var images =
                await _imageRepository
                    .GetImagesByGalleryIdAsync(galleryId);

            return await CompareAsync(
                referencePhoto,
                images,
                matchThreshold
            );
        }

        public async Task<List<MatchResultDto>> FindMatchesForUserAsync(
            Stream referencePhoto,
            int userId,
            double? matchThreshold = null
        )
        {
            var images =
                await _imageRepository
                    .GetImagesByUserIdAsync(userId);

            return await CompareAsync(
                referencePhoto,
                images,
                matchThreshold
            );
        }

        private async Task<List<MatchResultDto>> CompareAsync(
            Stream referencePhoto,
            List<Image> images,
            double? matchThreshold
        )
        {
            double threshold =
                matchThreshold
                ?? _engine.SuggestedMatchThreshold;

            if (threshold < 0 || threshold > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(matchThreshold),
                    "Threshold must be between 0 and 1."
                );
            }

            object reference =
                await _engine.ComputeDescriptorAsync(
                    referencePhoto
                );

            bool objectMode =
                _engine is YoloObjectMatchingService;

            string? referenceLabel = null;

            if (
                objectMode &&
                reference is YoloObjectMatchingService.Descriptor descriptor
            )
            {
                if (
                    descriptor.PrimaryClassId < 0 ||
                    descriptor.PrimaryConfidence < threshold
                )
                {
                    throw new InvalidDataException(
                        "No supported object was detected " +
                        "confidently in the reference photo."
                    );
                }

                referenceLabel =
                    descriptor.PrimaryLabel;
            }

            var results =
                new List<MatchResultDto>();

            foreach (var image in images)
            {
                var path =
                    ResolvePhysicalPath(image.Url);

                if (
                    path == null ||
                    !File.Exists(path)
                )
                {
                    continue;
                }

                try
                {
                    string? faceCoordinates = null;

                    if (!objectMode)
                    {
                        var detected =
                            await _detectionRepository
                                .GetDetectionsByImageIdAsync(
                                    image.Id
                                );

                        faceCoordinates =
                            detected
                                .OrderByDescending(
                                    d => d.Confidence
                                )
                                .FirstOrDefault()
                                ?.FaceCoordinates;
                    }

                    object candidate;

                    await using (
                        var imageStream =
                            File.OpenRead(path)
                    )
                    {
                        candidate =
                            await _engine.ComputeDescriptorAsync(
                                imageStream,
                                faceCoordinates
                            );
                    }

                    double score = Math.Clamp(
                        _engine.Compare(
                            reference,
                            candidate
                        ),
                        0,
                        1
                    );

                    string? note = null;

                    if (
                        objectMode &&
                        candidate is YoloObjectMatchingService.Descriptor
                            candidateInfo
                    )
                    {
                        note =
                            $"Reference: {referenceLabel}; " +
                            $"candidate: {candidateInfo.PrimaryLabel}";
                    }

                    results.Add(
                        new MatchResultDto
                        {
                            ImageId = image.Id,

                            GalleryId = image.GalleryId,

                            Url = image.Url,

                            ScorePercent =
                                Math.Round(
                                    score * 100,
                                    1
                                ),

                            IsMatch =
                                score >= threshold,

                            Note = note
                        }
                    );
                }
                catch (NoFaceDetectedException ex)
                {
                    results.Add(
                        new MatchResultDto
                        {
                            ImageId = image.Id,

                            GalleryId = image.GalleryId,

                            Url = image.Url,

                            ScorePercent = 0,

                            IsMatch = false,

                            Note = ex.Message
                        }
                    );
                }
                catch (
                    SixLabors.ImageSharp.UnknownImageFormatException
                )
                {
                    // Skip invalid images.
                }
            }

            return results
                .OrderByDescending(
                    r => r.ScorePercent
                )
                .ToList();
        }

        private string? ResolvePhysicalPath(
            string url
        )
        {
            if (
                string.IsNullOrWhiteSpace(url) ||
                string.IsNullOrWhiteSpace(_webRootPath)
            )
            {
                return null;
            }

            var relative =
                url
                    .TrimStart('/')
                    .Replace('\\', '/');

            if (
                !relative.StartsWith(
                    "uploads/",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return null;
            }

            var root =
                Path.GetFullPath(
                    Path.Combine(
                        _webRootPath,
                        "uploads"
                    )
                );

            var full =
                Path.GetFullPath(
                    Path.Combine(
                        _webRootPath,
                        relative.Replace(
                            '/',
                            Path.DirectorySeparatorChar
                        )
                    )
                );

            if (
                !full.StartsWith(
                    root + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return null;
            }

            return full;
        }
    }
}
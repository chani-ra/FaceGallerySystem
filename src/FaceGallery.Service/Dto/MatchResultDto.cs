namespace c__service_2026.Dto
{
    public class MatchResultDto
    {
        public int ImageId { get; set; }
        public int GalleryId { get; set; }
        public string Url { get; set; } = string.Empty;
        public double ScorePercent { get; set; }
        public bool IsMatch { get; set; }

        // Set when this image could not be compared (e.g. no face detected
        // in it, if the active IFaceMatchingService requires one) - ScorePercent
        // is 0 and IsMatch is false in that case, not a silent wrong answer.
        public string? Note { get; set; }
    }
}

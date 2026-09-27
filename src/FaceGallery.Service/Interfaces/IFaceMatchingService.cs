using System.IO;
using System.Threading.Tasks;

namespace c__service_2026.Interfaces
{
    /// <summary>
    /// Turns a photo into a "descriptor" (an opaque fingerprint object - its
    /// concrete shape is an implementation detail) and compares two
    /// descriptors for similarity. This is the seam that lets the whole
    /// recognition pipeline (RecognitionService, the controllers, the DTOs)
    /// stay unchanged while the underlying matching technology is swapped.
    ///
    /// Two implementations exist (see FaceGallery.Service/Services):
    /// - FaceAiSharpMatchingService (default): real face recognition - detects
    ///   the face, aligns it, and generates a deep-learning embedding
    ///   (ArcFace, via the FaceAiSharp library / ONNX Runtime), fully offline.
    /// - ImageSimilarityService: a simple local color-layout descriptor with
    ///   no face-detection requirement at all - kept as a fallback/demo path
    ///   that also works on non-photographic images. See docs/ARCHITECTURE.md.
    /// </summary>
    public interface IFaceMatchingService
    {
        /// <summary>
        /// Computes a descriptor for the given image stream. May throw
        /// NoFaceDetectedException (see FaceAiSharpMatchingService) if the
        /// active implementation requires a detectable face and none was found.
        /// faceCoordinates ("x,y,width,height") is an optional hint from a
        /// prior detection step; implementations that detect faces themselves
        /// may ignore it.
        /// </summary>
        Task<object> ComputeDescriptorAsync(Stream imageStream, string? faceCoordinates = null);

        /// <summary>
        /// Similarity between two descriptors produced by THIS SAME implementation.
        /// Range and meaning are implementation-specific - see SuggestedMatchThreshold.
        /// </summary>
        double Compare(object descriptorA, object descriptorB);

        /// <summary>
        /// The similarity score (in this implementation's own scale) above which
        /// two photos should be considered the same person. Callers should use
        /// this instead of hard-coding a threshold, so the system stays correct
        /// if the implementation is swapped.
        /// </summary>
        double SuggestedMatchThreshold { get; }
    }
}

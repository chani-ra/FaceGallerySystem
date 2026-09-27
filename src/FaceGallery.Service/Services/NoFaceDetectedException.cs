using System;

namespace c__service_2026.Services
{
    /// <summary>
    /// Thrown by FaceAiSharpMatchingService when a photo contains no
    /// detectable face. Callers (RecognitionService, controllers) catch this
    /// to give a friendly response instead of a generic 500 error.
    /// </summary>
    public class NoFaceDetectedException : Exception
    {
        public NoFaceDetectedException() : base("No face was detected in this image.") { }
        public NoFaceDetectedException(string message) : base(message) { }
    }
}

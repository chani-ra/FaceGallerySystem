// הנתיב המלא: C#-2026-service/Interfaces/IAzureVisionService.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace c__service_2026.Interfaces
{
    public interface IAzureVisionService
    {
        Task<List<FaceDetectionResult>> DetectFacesAsync(string imageUrl);
    }

    // מחלקה קטנה לעזרה בהעברת נתוני הזיהוי
    public class FaceDetectionResult
    {
        public double Confidence { get; set; }
        public string FaceCoordinates { get; set; } = string.Empty;
    }
}
// הנתיב המלא: C#-2026-service/Dto/DetectedCharacterDto.cs
using System;

namespace c__service_2026.Dto
{
    public class DetectedCharacterDto
    {
        public int Id { get; set; }
        public int ImageId { get; set; }
        public int CharacterId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public string FaceCoordinates { get; set; } = string.Empty;
        public DateTime DetectionDate { get; set; }
        public string ModelUsed { get; set; } = string.Empty;
    }
}
/*
 * מטרת העמוד:
 * מייצג זיהוי בודד של דמות בתוך תמונה.
 * הוספנו כאן את 'CharacterName' כדי שלצד-לקוח יהיה קל להציג את שם הדמות מיד,
 * בלי צורך לבצע קריאת שרת נוספת כדי לשלוף את השם לפי ה-ID.
 */
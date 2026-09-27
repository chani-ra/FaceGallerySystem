// הנתיב המלא: C#-2026-service/Dto/ImageDto.cs
using System;
using System.Collections.Generic;

namespace c__service_2026.Dto
{
    public class ImageDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int GalleryId { get; set; }
        public int UserId { get; set; }
        public DateTime UploadedDate { get; set; }

        // שדות לוגיים לממשק המשתמש:
        public bool IsProcessed { get; set; }
        public int DetectionCount { get; set; }
        public List<DetectedCharacterDto> Detections { get; set; } = new List<DetectedCharacterDto>();
    }
}
/*
 * מטרת העמוד:
 * מודל הנתונים של תמונה.
 * הוספו השדות: IsProcessed, DetectionCount ו-Detections.
 * שדות אלו קריטיים לאתר מקצועי, כדי שהמשתמש יוכל לראות מיד כשהוא פותח תמונה
 * אם היא כבר עברה סריקת AI, כמה פרצופים נמצאו בה, ומהם בדיוק הזיהויים.
 */
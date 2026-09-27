// הנתיב המלא: C#-2026-service/Dto/CharacterDto.cs
using System;

namespace c__service_2026.Dto
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TotalDetections { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
    }
}
/*
 * מטרת העמוד:
 * אובייקט להעברת נתוני דמות אל צד הלקוח. 
 * השדה 'TotalDetections' (סך כל הזיהויים) מחושב דינמית בשכבת הסרביס, 
 * כדי להעשיר את המידע שמוצג למשתמש מבלי לשמור אותו פיזית בטבלת הדמויות במסד הנתונים.
 */
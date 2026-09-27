// הנתיב המלא: C#-2026-service/Dto/GalleryDto.cs
using System;
using System.Collections.Generic;

namespace c__service_2026.Dto
{
    public class GalleryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CharacterId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }

        // שדות לוגיים:
        public int ImageCount { get; set; }
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
    }
}
/*
 * מטרת העמוד:
 * אובייקט להעברת פרטי הגלריה.
 * מביא איתו את כמות התמונות בגלריה (ImageCount) וכן את רשימת התמונות המלאה עצמה (Images),
 * כך בלחיצה על גלריה באתר הלקוח יקבל מיד את כל התוכן שלה בצורה חלקה ומהירה.
 */
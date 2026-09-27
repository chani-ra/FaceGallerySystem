// הנתיב המלא: C#-2026-service/Dto/UserDto.cs
using System;

namespace c__service_2026.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
/*
 * מטרת העמוד (UserDto):
 * אובייקט להעברת נתוני המשתמש (לקוח) בין השרת לצד הלקוח (האתר).
 * משמש בעיקר לתהליכי הרשמה והתחברות (Login/Register), 
 * כדי שנוכל לנהל הרשאות ולדעת איזו גלריה שייכת לאיזה משתמש.
 */
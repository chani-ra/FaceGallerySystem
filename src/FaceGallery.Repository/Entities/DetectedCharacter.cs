// הנתיב המלא: C#-nRepository-2026/Entities/DetectedCharacter.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// ודאי שהשורה הזו קיימת - היא אומרת לו להסתכל בתוך התיקייה של עצמו
namespace c__nRepository_2026.Entities
{
    public class DetectedCharacter
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ImageId { get; set; }

        [Required]
        public int CharacterId { get; set; }

        [Required]
        [Range(0.0, 1.0, ErrorMessage = "רמת הביטחון (Confidence) חייבת להיות בין 0 ל-1")]
        public float Confidence { get; set; }

        // --- הנה השדה שהיה חסר לנו! כאן נשמור את מיקום הפנים בתמונה ---
        [MaxLength(100)]
        public string FaceCoordinates { get; set; } = string.Empty;

        [Required]
        public DateTime DetectionDate { get; set; }

      [ForeignKey(nameof(ImageId))]
        public virtual c__nRepository_2026.Entities.Image Image { get; set; }

        [ForeignKey(nameof(CharacterId))]
        public virtual Character Character { get; set; }
    }
}

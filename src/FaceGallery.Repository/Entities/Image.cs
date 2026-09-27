//C#-nRepository-2026/Entities/Image.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace c__nRepository_2026.Entities
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "נתיב/כתובת התמונה הוא חובה")]
        [MaxLength(1000)]
        public string Url { get; set; } = string.Empty;

        [Required]
        public int GalleryId { get; set; }

        [ForeignKey(nameof(GalleryId))]
        public virtual Gallery Gallery { get; set; }

        public virtual ICollection<DetectedCharacter> DetectedCharacters { get; set; } = new List<DetectedCharacter>();

        [Required]
        public int UserId { get; set; }
    }
}

//C#-nRepository-2026/Entities/Gallery.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace c__nRepository_2026.Entities
{
    public class Gallery
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "שם הגלריה הוא שדה חובה")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int CharacterId { get; set; }


        [ForeignKey(nameof(CharacterId))]
        public virtual Character Character { get; set; }

        public virtual ICollection<Image> Images { get; set; } = new List<Image>();

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }
    }
}

//C#-nRepository-2026/Entities/Character.cs
using c__nRepository_2026.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace c__nRepository_2026.Entities
{
    public class Character
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "שם הדמות הוא שדה חובה")]
        [MaxLength(100, ErrorMessage = "שם הדמות לא יכול להכיל יותר מ-100 תווים")]
        public string CharacterName { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "התיאור ארוך מדי")]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<DetectedCharacter> DetectedCharacters { get; set; } = new List<DetectedCharacter>();
    }
}

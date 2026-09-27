// הנתיב המלא: C#-2026-service/Profiles/MappingProfile.cs
using AutoMapper;
using c__nRepository_2026.Entities;
using c__service_2026.Dto;

namespace c__service_2026.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ReverseMap מאפשר מיפוי דו-כיווני (מה-DB ללקוח, ומהלקוח ל-DB)
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<Character, CharacterDto>().ReverseMap();

            CreateMap<DetectedCharacter, DetectedCharacterDto>().ReverseMap();

            CreateMap<Gallery, GalleryDto>().ReverseMap();

            CreateMap<Image, ImageDto>().ReverseMap();
        }
    }
}
/*
 * מטרת העמוד:
 * הגדרת כללי המיפוי עבור סריית ה-AutoMapper.
 * פה אנחנו אומרים למערכת: "תלמדי להעביר נתונים אוטומטית בין טבלאות המסד (Entities) 
 * לבין האובייקטים שמוצגים ללקוח (DTOs)". זה חוסך לנו המון שורות קוד מיותרות בסרביס!
 */

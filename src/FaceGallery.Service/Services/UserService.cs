// הנתיב המלא: C#-2026-service/Services/UserService.cs
using AutoMapper;
using c__nRepository_2026.Entities;
using c__nRepository_2026.Interfaces;
using c__service_2026.Dto;
using c__service_2026.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace c__service_2026.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper; // הזרקת ה-AutoMapper!

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<List<UserDto>>(users); // קסם ה-AutoMapper בשורה אחת
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> AddItemAsync(UserDto item)
        {
            // המרה מהלקוח למסד הנתונים
            var userEntity = _mapper.Map<User>(item);
            var result = await _userRepository.AddItemAsync(userEntity);

            // המרה חזרה ללקוח
            return _mapper.Map<UserDto>(result);
        }

        public async Task UpdateItemAsync(int id, UserDto item)
        {
            var userEntity = _mapper.Map<User>(item);
            await _userRepository.UpdateItemAsync(id, userEntity);
        }

        public async Task DeleteItemAsync(int id)
        {
            await _userRepository.DeleteItemAsync(id);
        }

        // --- לוגיקה עסקית אמיתית של משתמשים ---

        public async Task<UserDto> RegisterAsync(UserDto userDto)
        {
            // 1. בדיקה שהנתונים תקינים
            if (string.IsNullOrWhiteSpace(userDto.Email) || string.IsNullOrWhiteSpace(userDto.Password))
                throw new ArgumentException("אימייל וסיסמה הם שדות חובה להרשמה.");

            // 2. בדיקה האם האימייל כבר קיים במערכת (מקצועיות!)
            var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUser != null)
                throw new Exception("כבר קיים משתמש עם כתובת האימייל הזו במערכת.");

            // 3. שמירה במסד
            var newUser = _mapper.Map<User>(userDto);
            var result = await _userRepository.AddItemAsync(newUser);
            return _mapper.Map<UserDto>(result);
        }

        public async Task<UserDto> LoginAsync(string emailOrUsername, string password)
        {
            var user = await _userRepository.GetByEmailAsync(emailOrUsername)
                    ?? await _userRepository.GetByUsernameAsync(emailOrUsername);

            // בדיקת התחברות
            if (user == null || user.Password != password)
            {
                throw new UnauthorizedAccessException("שם משתמש או סיסמה שגויים.");
            }

            return _mapper.Map<UserDto>(user);
        }
    }
}

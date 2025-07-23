using Microsoft.Win32;
using System.Security.Cryptography;
using System.Text;
using TresDos.Application.DTOs.ProductDto;
using TresDos.Application.DTOs.UserDto;
using TresDos.Application.Interfaces;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;

namespace TresDos.Application.UseCases
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository repo, ILogger<UserService> logger)
        {
            _repo = repo;
            _logger = logger;
        }
        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            var user = await _repo.GetByUsernameAsync(username);
            return user == null ? null : new UserDto { Username = user.Username, Password = user.PasswordHash};

            var user = await _service.GetByUsernameAsync(dto.Username);
            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var token = _tokenService.CreateToken(user);
        }
       
        public Task Register(RegisterUserDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                Role = dto.Role,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                CommissionPercentage = dto.CommissionPercentage,
                ParentId = dto.ParentId,
                Status = dto.Status
            };
            await _repo.AddAsync(product);
            throw new NotImplementedException();
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        private static bool VerifyPassword(string password, string hash)
            => HashPassword(password) == hash;
    }
}

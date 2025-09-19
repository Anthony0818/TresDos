using MediatR;
using System.Security.Cryptography;
using System.Text;
using TresDos.Application.DTOs.UserDto;
using TresDos.Application.Feature.Users.Commands;
using TresDos.Core.Interfaces;
using TresDos.Services;

namespace TresDos.Application.Feature.Users.CommandsHandler
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IUserRepository _repo;
        private readonly ITokenService _tokenService;

        public LoginHandler(IUserRepository repo, ITokenService tokenService)
        {
            _repo = repo;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _repo.GetByUsernameAsync(request.Username);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var userDto = new UserDto
            {
                Username = user.Username,
                Role = user.Role
            };

            var token = _tokenService.CreateToken(userDto);

            return new LoginResponseDto
            {
                Token = token,
                UserDetail = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    CommissionPercentage = user.CommissionPercentage,
                    ParentId = user.ParentId,
                    Role = user.Role,
                }
            };
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

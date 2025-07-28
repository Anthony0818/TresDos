using MediatR;
using System.Security.Cryptography;
using System.Text;
using TresDos.Application.DTOs.UserDto;
using TresDos.Application.Feature.Users.Commands;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.Users.CommandsHandler
{
    public class LoginHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IUserRepository _repo;
        private readonly TokenService _tokenService;

        public LoginHandler(IUserRepository repo, TokenService tokenService)
        {
            _repo = repo;
            _tokenService = tokenService;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
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

            return token;
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

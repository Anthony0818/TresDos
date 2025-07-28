using AutoMapper;
using MediatR;
using System.Security.Cryptography;
using System.Text;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;
using TresDos.Feature.Users.Commands;

namespace TresDos.Feature.Users.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, User>
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public RegisterUserHandler(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<User> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Username and password cannot be empty");
            }
            if (await _repo.GetByUsernameAsync(request.Username) != null)
            {
                throw new InvalidOperationException("Username already exists");
            }
            var user = _mapper.Map<User>(request);
            user.PasswordHash = HashPassword(request.Password);
            user.Role = "User"; // Default role, can be changed later
            await _repo.Register(user);
            return user;
        }
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}

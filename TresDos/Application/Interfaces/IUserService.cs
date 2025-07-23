using TresDos.Application.DTOs.UserDto;

namespace TresDos.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByUsernameAsync(string username);
        Task Register(RegisterUserDto dto);
    }
}

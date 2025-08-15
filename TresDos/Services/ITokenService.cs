using TresDos.Application.DTOs.UserDto;

namespace TresDos.Services
{
    public interface ITokenService
    {
        string CreateToken(UserDto user);
    }
}

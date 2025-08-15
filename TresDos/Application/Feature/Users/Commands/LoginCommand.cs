using MediatR;
using TresDos.Application.DTOs.UserDto;
namespace TresDos.Application.Feature.Users.Commands
{
    public record LoginCommand(string Username, string Password) : IRequest<LoginResponseDto>;
}

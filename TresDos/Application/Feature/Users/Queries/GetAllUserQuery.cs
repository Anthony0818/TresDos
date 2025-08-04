using MediatR;
using TresDos.Application.DTOs.UserDto;

namespace TresDos.Application.Feature.Users.Queries
{
    public record GetAllUserQuery : IRequest<List<UserDto>> { }
}

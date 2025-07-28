using MediatR;
using TresDos.Application.DTOs.UserDto;

namespace TresDos.Application.Feature.Products.Queries
{
    public record GetProductByUsernameQuery(string Username) : IRequest<UserDto>;
}

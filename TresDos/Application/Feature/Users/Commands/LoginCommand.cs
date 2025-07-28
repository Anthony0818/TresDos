using MediatR;
namespace TresDos.Application.Feature.Users.Commands
{
    public record LoginCommand(string Username, string Password) : IRequest<string>;
}

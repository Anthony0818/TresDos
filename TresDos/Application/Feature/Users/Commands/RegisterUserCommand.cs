using MediatR;
using TresDos.Core.Entities;
namespace TresDos.Feature.Users.Commands
{
    public record RegisterUserCommand(
        string Username, 
        string Password, 
        string Role,
        string FirstName,
        string MiddleName,
        string LastName,
        int? CommissionPercentage,
        int? ParentId,
        bool? Status
        ) : IRequest<User>;
}

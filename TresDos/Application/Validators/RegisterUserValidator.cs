using FluentValidation;
using TresDos.Feature.Users.Commands;

namespace TresDos.Application.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator() {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.Role).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.CommissionPercentage).NotEmpty();
            RuleFor(x => x.Status).NotEmpty();
        }
    }
}

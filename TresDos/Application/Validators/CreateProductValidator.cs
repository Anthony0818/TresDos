using FluentValidation;
using TresDos.Application.Feature.Products.Commands;

namespace TresDos.Application.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator() {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Price).NotEmpty();
        }
    }
}

using MediatR;

namespace TresDos.Application.Feature.Products.Commands
{
    public record UpdateProductCommand(int Id, string Name, string Description, decimal Price) : IRequest<Unit>;
}
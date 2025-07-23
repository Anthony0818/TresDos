using MediatR;

namespace TresDos.Application.Feature.Products.Commands
{
    public record UpdateProductCommand(int Id, string Name, decimal Price) : IRequest<Unit>;
}
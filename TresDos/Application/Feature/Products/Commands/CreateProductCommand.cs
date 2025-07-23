using MediatR;
namespace TresDos.Application.Feature.Products.Commands
{
    public record CreateProductCommand(string Name, decimal Price) : IRequest<int>;
}

using MediatR;
using TresDos.Core.Entities;
namespace TresDos.Application.Feature.Products.Commands
{
    public record CreateProductCommand(string Name, string Description, decimal Price) : IRequest<Product>;
}

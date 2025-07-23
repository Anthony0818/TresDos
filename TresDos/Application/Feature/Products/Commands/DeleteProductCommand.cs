using MediatR;

namespace TresDos.Application.Feature.Products.Commands
{
    public record DeleteProductCommand : IRequest<Unit>
    {
        public int Id { get; init; }
    }
}

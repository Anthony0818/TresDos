using MediatR;
using TresDos.Application.Feature.Products.Commands;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.Products.Handlers
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IProductRepository _repo;

        public DeleteProductHandler(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repo.GetByIdAsync(request.Id)
                          ?? throw new KeyNotFoundException("Product not found");

            await _repo.DeleteAsync(product.Id);
            return Unit.Value;
        }
    }
}

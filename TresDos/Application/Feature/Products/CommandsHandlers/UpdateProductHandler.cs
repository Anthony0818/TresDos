using MediatR;
using TresDos.Application.Feature.Products.Commands;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.Products.Handlers
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IProductRepository _repo;


        public UpdateProductHandler(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repo.GetByIdAsync(request.Id)
                          ?? throw new KeyNotFoundException("Product not found");

            product.Validate(request.Name, request.Price);
            await _repo.UpdateAsync(product);
            return Unit.Value;
        }
    }
}

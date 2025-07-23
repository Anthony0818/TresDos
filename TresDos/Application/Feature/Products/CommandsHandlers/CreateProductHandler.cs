using AutoMapper;
using MediatR;
using TresDos.Application.Feature.Products.Commands;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.Products.Handlers
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public CreateProductHandler(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request);
            await _repo.AddAsync(product);
            return product.Id;
        }
    }
}

using AutoMapper;
using MediatR;
using TresDos.Application.Feature.Products.Commands;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.Products.Handlers
{
    public class RegisterUserHandler : IRequestHandler<CreateProductCommand, Product>
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public RegisterUserHandler(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = _mapper.Map<Product>(request);
            await _repo.AddAsync(product);
            return product;
        }
    }
}

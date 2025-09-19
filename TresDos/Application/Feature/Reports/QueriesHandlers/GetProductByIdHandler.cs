using AutoMapper;
using MediatR;
using TresDos.Application.DTOs.ProductDto;
using TresDos.Application.Feature.Products.Queries;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.Reports.QueriesHandlers
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repo.GetByIdAsync(request.Id)
                          ?? throw new KeyNotFoundException("Product not found");

            return _mapper.Map<ProductDto>(product);
        }
    }
}

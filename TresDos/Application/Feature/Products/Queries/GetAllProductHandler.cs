using AutoMapper;
using MediatR;
using TresDos.Application.DTOs.ProductDto;
using TresDos.Application.DTOs.UserDto;
using TresDos.Core.Interfaces;
using TresDos.Infrastructure.Data;

namespace TresDos.Application.Feature.Products.Queries
{
    public class GetAllProductHandler : IRequestHandler<GetAllProductQuery, List<ProductDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _repo;

        public GetAllProductHandler(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var products = await _repo.GetAllAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}

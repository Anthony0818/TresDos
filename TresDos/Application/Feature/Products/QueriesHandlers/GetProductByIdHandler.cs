using AutoMapper;
using MediatR;
using TresDos.Application.DTOs.ProductDto;
using TresDos.Application.DTOs.UserDto;
using TresDos.Application.Feature.Products.Queries;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.Products.QueriesHandlers
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, TwoDDto>
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public GetProductByIdHandler(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<TwoDDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _repo.GetByIdAsync(request.Id)
                          ?? throw new KeyNotFoundException("Product not found");

            return _mapper.Map<TwoDDto>(product);
        }
    }
}

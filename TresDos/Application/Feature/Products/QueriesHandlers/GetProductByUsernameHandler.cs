using AutoMapper;
using MediatR;
using TresDos.Application.DTOs.ProductDto;
using TresDos.Application.DTOs.UserDto;
using TresDos.Application.Feature.Products.Queries;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.Products.QueriesHandlers
{
    public class GetProductByUsernameHandler : IRequestHandler<GetProductByUsernameQuery, UserDto>
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public GetProductByUsernameHandler(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetProductByUsernameQuery request, CancellationToken cancellationToken)
        {
            var user = await _repo.GetByUsernameAsync(request.Username)
                          ?? throw new KeyNotFoundException("User not found");

            return _mapper.Map<UserDto>(user);
        }
    }
}

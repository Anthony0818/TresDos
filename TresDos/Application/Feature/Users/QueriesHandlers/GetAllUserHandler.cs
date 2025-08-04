using AutoMapper;
using MediatR;
using TresDos.Application.DTOs.UserDto;
using TresDos.Application.Feature.Users.Queries;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.Users.QueriesHandlers
{
    public class GetAllUserHandler : IRequestHandler<GetAllUserQuery, List<UserDto>>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _repo;

        public GetAllUserHandler(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var users = await _repo.GetAllAsync();
            return _mapper.Map<List<UserDto>>(users);
        }
    }
}

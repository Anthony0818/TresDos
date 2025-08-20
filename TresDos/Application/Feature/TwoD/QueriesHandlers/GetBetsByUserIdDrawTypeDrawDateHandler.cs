using AutoMapper;
using MediatR;
using TresDos.Application.DTOs.BetDto;
using TresDos.Application.Feature.TwoD.Queries;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.TwoD.QueriesHandlers
{
    public class GetBetsByUserIdDrawTypeDrawDateHandler : IRequestHandler<GetBetsByUserIdDrawTypeDrawDateQuery, List<TwoDBetsDto>>
    {
        private readonly IMapper _mapper;
        private readonly ITwoDRepository _repo;

        public GetBetsByUserIdDrawTypeDrawDateHandler(ITwoDRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<List<TwoDBetsDto>> Handle(GetBetsByUserIdDrawTypeDrawDateQuery request, CancellationToken cancellationToken)
        {
            var bets = await _repo.GetBetsByUserIdDrawTypeDrawDate(request.userId,request.drawType, request.drawDate);
            return bets ?? new List<TwoDBetsDto>();
        }
    }
}

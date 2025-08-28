using AutoMapper;
using MediatR;
using TresDos.Application.DTOs.BetDto;
using TresDos.Application.Feature.TwoD.Queries;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.TwoD.QueriesHandlers
{
    public class GetTwoDWinnersHandler : IRequestHandler<GetTwoDWinnersQuery, List<TwoDWinResultDto>>
    {
        private readonly IMapper _mapper;
        private readonly ITwoDRepository _repo;

        public GetTwoDWinnersHandler(ITwoDRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<List<TwoDWinResultDto>> Handle(GetTwoDWinnersQuery request, CancellationToken cancellationToken)
        {
            var winners = await _repo.GetTwoDWinnerByDrawTypeAndDate(request.drawType, request.drawDate, request.firstDigit,request.secondDigit);
            return winners ?? new List<TwoDWinResultDto>();
        }
    }
}

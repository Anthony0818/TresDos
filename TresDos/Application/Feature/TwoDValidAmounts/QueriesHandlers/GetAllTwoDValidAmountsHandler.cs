using AutoMapper;
using MediatR;
using TresDos.Application.Feature.DrawSettings.Queries;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.DrawSettings.QueriesHandlers
{
    public class GetAllTwoDValidAmountsHandler : IRequestHandler<GetAllTwoDValidAmountsQuery, List<ltb_twoDValidAmounts>>
    {
        private readonly IMapper _mapper;
        private readonly IDrawSetingsRepository _repo;

        public GetAllTwoDValidAmountsHandler(IDrawSetingsRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<ltb_twoDValidAmounts>> Handle(GetAllTwoDValidAmountsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repo.GetAllAsync();
            return _mapper.Map<List<ltb_twoDValidAmounts>>(products);
        }
    }
}

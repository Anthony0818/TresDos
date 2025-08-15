using AutoMapper;
using MediatR;
using TresDos.Application.Feature.DrawSettings.Queries;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.DrawSettings.QueriesHandlers
{
    public class GetAllDrawSettingsHandler : IRequestHandler<GetAllDrawSettingsQuery, List<ltb_DrawSettings>>
    {
        private readonly IMapper _mapper;
        private readonly IDrawSetingsRepository _repo;

        public GetAllDrawSettingsHandler(IDrawSetingsRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<ltb_DrawSettings>> Handle(GetAllDrawSettingsQuery request, CancellationToken cancellationToken)
        {
            var products = await _repo.GetAllAsync();
            return _mapper.Map<List<ltb_DrawSettings>>(products);
        }
    }
}

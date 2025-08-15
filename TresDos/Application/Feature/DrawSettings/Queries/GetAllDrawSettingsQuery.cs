using MediatR;
using TresDos.Core.Entities;

namespace TresDos.Application.Feature.DrawSettings.Queries
{
    public record GetUserBetEntiesQuery : IRequest<List<ltb_DrawSettings>> { }
}

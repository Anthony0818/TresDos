using MediatR;
using TresDos.Core.Entities;

namespace TresDos.Application.Feature.DrawSettings.Queries
{
    public record GetAllDrawSettingsQuery : IRequest<List<ltb_DrawSettings>> { }
}

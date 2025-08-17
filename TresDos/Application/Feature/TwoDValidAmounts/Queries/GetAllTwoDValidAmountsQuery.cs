using MediatR;
using TresDos.Core.Entities;

namespace TresDos.Application.Feature.DrawSettings.Queries
{
    public record GetAllTwoDValidAmountsQuery : IRequest<List<ltb_twoDValidAmounts>> { }
}

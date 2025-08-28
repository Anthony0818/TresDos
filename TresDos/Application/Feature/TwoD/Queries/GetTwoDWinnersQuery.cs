using MediatR;
using TresDos.Application.DTOs.BetDto;
using TresDos.Core.Entities;

namespace TresDos.Application.Feature.TwoD.Queries
{
    public record GetTwoDWinnersQuery(
       string drawType, 
       DateTime drawDate,
       int firstDigit,
       int secondDigit
        ) : IRequest<List<TwoDWinResultDto>> { }
}

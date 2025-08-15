using MediatR;
using TresDos.Core.Entities;

namespace TresDos.Application.Feature.TwoD.Queries
{
    public record GetBetsByUserIdDrawTypeDrawDateQuery(
       int userId, 
       string drawType, 
       DateTime drawDate
        ) : IRequest<List<tb_TwoD>> { }
}

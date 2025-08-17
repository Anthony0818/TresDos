using MediatR;
using TresDos.Application.DTOs.BetDto;
namespace TresDos.Application.Feature.TwoD.Commands
{
   public record BulkValidateTwoDCommand(
         BulkValidateTwoDEntriesRequestDto requestDto
       ) : IRequest<List<BulkValidateTwoDEntriesResultDto>>;
}

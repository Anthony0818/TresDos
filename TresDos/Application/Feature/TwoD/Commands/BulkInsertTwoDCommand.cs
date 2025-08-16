using MediatR;
using TresDos.Application.DTOs.BetDto;
using TresDos.Core.Entities;
namespace TresDos.Application.Feature.TwoD.Commands
{
   public record BulkInsertTwoDCommand(
         BulkInsertTwoDEntriesRequestDto requestDto
       ) : IRequest<(List<tb_TwoD>, List<BulkInsertTwoDEntriesProcessingResultDto>)>;
}

using MediatR;
using TresDos.Application.DTOs.BetDto;

namespace TresDos.Application.Feature.TwoD.Commands
{
    public record BulkDeleteTwoDCommand(
        BulkDeleteEntriesRequest requestDto
        ) : IRequest<Unit>;
   
}

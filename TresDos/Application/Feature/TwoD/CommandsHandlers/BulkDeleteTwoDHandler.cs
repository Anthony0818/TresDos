using MediatR;
using TresDos.Application.Feature.TwoD.Commands;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.TwoD.Handlers
{
    public class BulkDeleteTwoDHandler : IRequestHandler<BulkDeleteTwoDCommand, Unit>
    {
        private readonly ITwoDRepository _repo;

        public BulkDeleteTwoDHandler(ITwoDRepository repo)
        {
            _repo = repo;
        }

        public async Task<Unit> Handle(BulkDeleteTwoDCommand request, CancellationToken cancellationToken)
        {
            await _repo.RemoveEntriesAsync(request.requestDto);
            return Unit.Value;
        }
    }
}

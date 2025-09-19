using AutoMapper;
using MediatR;
using TresDos.Application.DTOs.Reports;
using TresDos.Application.Feature.Reports.Queries;
using TresDos.Core.Interfaces;

namespace TresDos.Application.Feature.Reports.QueriesHandlers
{
    public class GetAllUserSalesHandler : IRequestHandler<GetAllUserSalesQuery, List<SalesReportResponseDTO>>
    {
        private readonly ISalesReportRepository _repo;

        public GetAllUserSalesHandler(ISalesReportRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<SalesReportResponseDTO>> Handle(GetAllUserSalesQuery request, CancellationToken cancellationToken)
        {
            var result = await _repo.GetAllUsersSalesReport(request.DrawDate);
            return result.ToList();
        }
    }
}

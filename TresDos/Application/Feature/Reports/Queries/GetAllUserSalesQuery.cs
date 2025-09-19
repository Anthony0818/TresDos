using MediatR;
using TresDos.Application.DTOs.Reports;

namespace TresDos.Application.Feature.Reports.Queries
{
    public record GetAllUserSalesQuery(DateTime DrawDate) : IRequest<List<SalesReportResponseDTO>> { }
}

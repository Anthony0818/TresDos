using MediatR;
using TresDos.Application.DTOs.Reports;

namespace TresDos.Application.Reports.Products.Queries
{
    public record GetAllUserSalesByUserIdQuery(int Id) : IRequest<SalesReportResponseDTO>;
}



using TresDos.Application.DTOs.Reports;

namespace TresDos.Core.Interfaces
{
    public interface ISalesReportRepository
    {
        Task<IEnumerable<SalesReportResponseDTO>> GetAllUsersSalesReport(string UserId,DateTime DrawDate);
    }
}


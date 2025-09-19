using Microsoft.EntityFrameworkCore;
using TresDos.Application.DTOs.Reports;
using TresDos.Core.Interfaces;
using TresDos.Infrastructure.Data;

namespace TresDos.Infrastructure.Repositories
{
    public class SalesReportRepository : ISalesReportRepository
    {
        private readonly AppDbContext _context;

        public SalesReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalesReportResponseDTO>> GetAllUsersSalesReport(DateTime DrawDate)
        {
            var result = await _context.tb_TwoD
                .Where(t => t.DrawDate.Date == DrawDate)
                //.Where(t => t.UserId == userId) // Uncomment to filter by user
                .GroupBy(t => new { t.User.Id, t.User.Username, t.User.CommissionPercentage })
                .Select(g => new SalesReportResponseDTO
                {
                    UserId = g.Key.Id,
                    Username = g.Key.Username,
                    TwoD2PM = g.Where(t => t.DrawType == "2D 2PM Draw").Sum(t => (decimal?)t.Amount) ?? 0,
                    TwoD5PM = g.Where(t => t.DrawType == "2D 5PM Draw").Sum(t => (decimal?)t.Amount) ?? 0,
                    TwoD9PM = g.Where(t => t.DrawType == "2D 9PM Draw").Sum(t => (decimal?)t.Amount) ?? 0,
                    TotalSales = g.Sum(t => (decimal?)t.Amount) ?? 0,
                    CommissionPercentage = g.Key.CommissionPercentage,
                    Commission = (g.Sum(t => (decimal?)t.Amount) ?? 0) * (g.Key.CommissionPercentage / 100.0m)
                })
                .OrderBy(x => x.Username)
                .ToListAsync(); // 🧠 Use async EF method

            return result;
        }
    }
}
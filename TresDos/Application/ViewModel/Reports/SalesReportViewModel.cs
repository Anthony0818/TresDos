
using TresDos.Core.Entities;

namespace TresDos.Application.DTOs.Reports
{
    public class SalesReportViewModel
    {
        public DateTime DrawDate { get; set; }
        public int UserId { get; set; }
        public List<SalesReportResponseDTO> SalesPerUser { get; set; } = new List<SalesReportResponseDTO>();
        public List<User> Users { get; set; } = new List<User>();
    }
}

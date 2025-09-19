namespace TresDos.Application.DTOs.Reports
{
    public class SalesReportResponseDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public decimal TwoD2PM { get; set; }
        public decimal TwoD5PM { get; set; }
        public decimal TwoD9PM { get; set; }
        public decimal TotalSales { get; set; }
        public int? CommissionPercentage { get; set; }
        public decimal? Commission { get; set; }
    }
}

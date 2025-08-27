namespace TresDos.Application.DTOs.BetDto
{
    public class TwoDWinResultDto
    {
        public Guid id { get; set; }
        public string Admin { get; set; }
        public string Bettor { get; set; }
        public int FirstDigit { get; set; }
        public int SecondDigit { get; set; }
        public string WinType { get; set; }
        public decimal Amount { get; set; }
        public decimal? WinPrize { get; set; }
        public DateTime DrawDate { get; set; }
        public string DrawType { get; set; }
    }
}

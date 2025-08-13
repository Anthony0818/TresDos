namespace TresDos.Application.DTOs.BetDto
{
    public class TwoDDto
    {
        public Guid id { get; set; }
        public string Bettor { get; set; }
        public int FirstDigit { get; set; }
        public int SecondDigit { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string DrawType { get; set; }
        public DateTime DrawDate { get; set; }
    }
    public class BulkInsertTwoDEntriesRequestDto
    {
        public List<TwoDDto> Entries { get; set; }
    }
    public class BulkInsertTwoDEntriesProcessingResultDto : TwoDDto
    {
        public bool IsInserted { get; set; }
        public string Message { get; set; }
        public decimal AvailableBalance { get; set; }
    }
}

namespace TresDos.Application.DTOs.BetDto
{
    public class TwoDBetsDto
    {
        public string Bettor { get; set; }
        public DateTime CreateDate { get; set; }
        public string DrawType { get; set; }
        public DateTime DrawDate { get; set; }

        public Guid id { get; set; }
        public int FirstDigit { get; set; }
        public int SecondDigit { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string AgentName { get; set; }
        public string Creator { get; set; }
    }
}

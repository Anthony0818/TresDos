namespace TresDos.Core.Entities
{
    public class tb_TwoD
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
        public int CreatedBy { get; set; }
    }
}

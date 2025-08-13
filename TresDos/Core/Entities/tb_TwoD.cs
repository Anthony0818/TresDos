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

        //public tb_TwoD(Guid _id, string _Bettor, int _FirstDigit, int _SecondDigit, string _Type, decimal _Amount, int _UserID, DateTime _CreateDate, string _DrawType, DateTime _DrawDate)
        //{
        //    this.id = id;
        //    this.Bettor = _Bettor;
        //    this.FirstDigit = _FirstDigit;
        //    this.SecondDigit = _SecondDigit;
        //    this.Type = _Type;
        //    this.Amount = _Amount;
        //    this.UserID = _UserID;
        //    this.CreateDate = _CreateDate;
        //    this.DrawType = _DrawType;
        //    this.DrawDate = _DrawDate;
        //}
    }
}

namespace TresDos.Core.Entities
{
    public class ltb_twoDValidAmounts
    {
        public int id { get; set; }
        public decimal Amount { get; set; }
        public decimal? WinStraight{ get; set; }
        public decimal? WinRamble { get; set; }
        public decimal? WinPompi { get; set; }
    }
}

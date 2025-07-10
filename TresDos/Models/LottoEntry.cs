namespace TresDos.Models
{
    public class LottoBatch
    {
        public List<LottoEntry> Entries { get; set; } = new List<LottoEntry>();
    }
    public class LottoEntry
    {
        public string BettorName { get; set; }
        public List<BetLine> Bets { get; set; } = new List<BetLine>();
    }

    public class BetLine
    {
        public string RawInput { get; set; }
        public string Bettor { get; set; }
        public string Combination { get; set; }
        public string Amount { get; set; }
        public string BetType { get; set; } // "S" or "R"
        public string Error { get; set; }   // Optional: for validation error

        public string BetTypeName =>
            BetType == "S" ? "Straight" :
            BetType == "R" ? "Random" :
            "Invalid";
    }
}

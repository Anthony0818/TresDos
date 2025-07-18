using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TresDos.Models
{
    public class LottoBatch
    {
        [Required(ErrorMessage = "Entry is required.")]
        public string Entry { get; set; }
        [Required(ErrorMessage = "Entry is required.")]
        //public string Agen { get; set; }
        public string SelectedAgent { get; set; }
        public List<SelectListItem> Agents { get; set; }
        public List<LottoEntry> Entries { get; set; } = new List<LottoEntry>();
    }
    public class LottoEntry
    {
        public required string BettorName { get; set; }
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

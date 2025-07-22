using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TresDos.Models
{
    public class LottoBatch
    {
        [Required(ErrorMessage = "Combination Entry is required.")]
        public string Entry { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Agent is required.")]
        public int SelectedAgentID { get; set; }
        public string SelectedAgentText { get; set; }
        public List<SelectListItem> Agents { get; set; }
        public List<LottoEntry> Entries { get; set; } = new List<LottoEntry>();
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime SelectedDate { get; set; }
        public string SelectedTime { get; set; }

        // Optional: list of options to bind to the dropdown
        public List<SelectListItem> TimeOptions { get; set; }

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
            (BetType == "S" || BetType == "s") ? "Straight" :
            (BetType == "R" || BetType == "r") ? "Random" :
            "Invalid";
    }
}

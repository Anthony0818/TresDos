using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TresDos.Application.ViewModel.BetModel
{
    public class TwoDViewModel
    {
        [Required(ErrorMessage = "Combination Entry is required.")]
        public string Entry { get; set; }
        [Required(ErrorMessage = "Select Agent.")]
        [Display(Name = "Agent")]
        public int SelectedAgentID { get; set; }
        public string SelectedAgentText { get; set; }
        public List<SelectListItem> Agents { get; set; }
        public List<Entry> Entries { get; set; } = new List<Entry>();
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DrawDate { get; set; }
        public string DrawType { get; set; }
        public TimeSpan DrawTime { get; set; }
        public TimeSpan DrawCutOffTime { get; set; }

        // Optional: list of options to bind to the dropdown
        public List<SelectListItem> TimeOptions { get; set; }
        public string ValidAmountsConcat { get; set; }

    }
    public class Entry
    {
        public string BettorName { get; set; }
        public List<BetLine> Bets { get; set; } = new List<BetLine>();
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string DrawType { get; set; }
        public DateTime DrawDate { get; set; }
        public int CreatedBy { get; set; }
    }

    public class BetLine
    {
        public Guid id { get; set; }
        public string RawInput { get; set; }
        public string Combination { get; set; }
        public int FirstDigit { get; set; }
        public int SecondDigit { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Error { get; set; }   // Optional: for validation error

        public string BetTypeName =>
            Type == "S" || Type == "s" ? "Straight" :
            Type == "R" || Type == "r" ? "Random" :
            "Invalid";
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TresDos.Application.DTOs.BetDto;

namespace TresDos.Application.ViewModel.BetModel
{
    public class TwoDWinnersViewModel
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DrawDate { get; set; }
        public TimeSpan DrawTime { get; set; }
        public TimeSpan DrawCutOffTime { get; set; }
        public string DrawType { get; set; }
        public bool IsBetAllowed { get; set; }
        public List<TwoDWinResultDto> Winners { get; set; } = new List<TwoDWinResultDto>();
        public List<SelectListItem> DrawTypeOptions { get; set; }
        public int FirstDigit { get; set; }
        public int SecondDigit { get; set; }
    }
}

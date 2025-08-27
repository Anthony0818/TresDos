using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TresDos.Application.DTOs.BetDto;
using TresDos.Application.DTOs.ProductDto;
using TresDos.Application.ViewModel.BetModel;
using TresDos.Core.Entities;
using TresDos.Helper;
using TresDos.Services;

namespace TresDos.Controllers.Web
{
    public class BetController : Controller
    {
        private readonly ICacheDrawSettings _drawSettings;
        private readonly ICacheTwoDValidAmount _twoDValidAmounts;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        DateTimeHelper _dateTimeHelper = new DateTimeHelper();
        //private static readonly List<decimal> ValidAmounts = Enumerable.Range(1, 60).Select(i => i * 5m).ToList(); // 10 to 300
        public BetController(
            IConfiguration configuration,
            IHttpClientFactory clientFactory, 
            ICacheDrawSettings drawSettings, 
            ICacheTwoDValidAmount twoDValidAmounts)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _drawSettings = drawSettings;
            _twoDValidAmounts = twoDValidAmounts;
        }

        #region Agents
        public async Task<List<SelectListItem>> GetAgentsUnderIncludingSelf(int? UserId)
        {
            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            var response = await client.GetAsync("api/usersapi");

            if (!response.IsSuccessStatusCode)
            {
                return new List<SelectListItem>(); // Return empty list on failure
            }

            var users = await response.Content.ReadFromJsonAsync<List<User>>();

            //var userList = users?.Select(u => new SelectListItem
            //{
            //    Text = (u.FirstName + " " + u.MiddleName + " " + u.LastName),
            //    Value = u.Id.ToString()
            //}).ToList() ?? new List<SelectListItem>();

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var userList = users?
                .Where(u => u.Id == userId)
                .Select(u => new SelectListItem
                {
                    Text = $"{u.FirstName} {u.MiddleName} {u.LastName}",
                    Value = u.Id.ToString(),
                    Selected = true
                }).ToList() ?? new List<SelectListItem>();

            userList.Insert(0, new SelectListItem
            {
                Text = "-- Select Agent --",
                Value = "0",
                Selected = true
            });
            return userList;
        }
        #endregion

        //#region 3D
        //[Route("Bet/3d")]
        //public ActionResult ThreeD()
        //{
        //    return View();
        //}
        //[Route("Bet/3d")]
        //[HttpPost]
        //public ActionResult ThreeD(string rawInput)
        //{
        //    var batch = new LottoBatch();
        //    var lines = rawInput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        //    LottoEntry currentEntry = null;
        //    HashSet<string> bettorNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        //    HashSet<string> localDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //    foreach (var line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
        //    {
        //        if (line.StartsWith("@"))
        //        {
        //            string bettorName = line.Substring(1).Trim();

        //            // Check for duplicate bettor
        //            if (bettorNames.Contains(bettorName))
        //            {
        //                currentEntry = new LottoEntry
        //                {
        //                    BettorName = bettorName + " (DUPLICATE)",
        //                };
        //                currentEntry.Bets.Add(new BetLine
        //                {
        //                    RawInput = line,
        //                    Error = $"Duplicate bettor name: {bettorName}"
        //                });
        //                batch.Entries.Add(currentEntry);
        //                continue;
        //            }

        //            bettorNames.Add(bettorName);
        //            currentEntry = new LottoEntry { BettorName = bettorName };
        //            batch.Entries.Add(currentEntry);
        //            localDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Reset for each bettor
        //            continue;
        //        }

        //        if (currentEntry == null)
        //            continue; // Skip lines if no bettor name yet

        //        var bet = ParseThreeDBetLine(line);
        //        string key = $"{NormalizeThreeDCombo(bet.Combination)}-{bet.BetType}";

        //        if (localDuplicates.Contains(key))
        //        {
        //            bet.Error = "Duplicate entry";
        //        }
        //        else
        //        {
        //            localDuplicates.Add(key);
        //        }

        //        currentEntry.Bets.Add(bet);
        //    }

        //    return View("ResultThreeD", batch);
        //}
        //private BetLine ParseThreeDBetLine(string line)
        //{
        //    var result = new BetLine
        //    {
        //        RawInput = line // Store original format
        //    };

        //    var match = Regex.Match(line, @"^(\d{3}|\d\s*-\s*\d\s*-\s*\d|\d\s*\*\s*\d\s*\*\s*\d|\d\s+\d\s+\d)\s*=\s*(\d{2,3})([SRsr])$");
        //    if (!match.Success)
        //    {
        //        result.Error = $"Invalid format: {line}";
        //        return result;
        //    }
        //    try
        //    {

        //        var combo = match.Groups[1].Value;
        //        var amountStr = match.Groups[2].Value;
        //        var type = match.Groups[3].Value.ToUpper();

        //        //result.Bettor = line.Substring(1).Trim();
        //        result.Combination = combo.Contains("-") ? combo : string.Join("-", combo.ToCharArray());
        //        result.Amount = amountStr;
        //        result.BetType = type;

        //        // Normalize combination
        //        string comboDigits = combo.Replace(" ", "").Replace("-", "");
        //        if (comboDigits.Length != 3)
        //        {
        //            result.Error = "Combination must contain exactly 3 digits.";
        //            return result;
        //        }

        //        result.Combination = string.Join("-", comboDigits.ToCharArray());
        //        result.BetType = type;

        //        // Amount parsing
        //        if (!int.TryParse(amountStr, out int amount))
        //        {
        //            result.Error = $"Invalid amount: {amountStr}";
        //            return result;
        //        }

        //        result.Amount = amount.ToString();

        //        // Validate amount is in list
        //        if (!ValidAmounts.Contains(amount))
        //        {
        //            result.Error = $"Amount {amount} is not allowed.";
        //            return result;
        //        }

        //        // Trio check
        //        if (IsTrio(combo) && type == "R")
        //        {
        //            result.Error = $"Trio combinations can only have straight bets (S): {line}";
        //        }

        //        // ✅ Check that all chars are digits 0–9
        //        if (!comboDigits.All(c => char.IsDigit(c) && c >= '0' && c <= '9'))
        //        {
        //            result.Error = "Only digits 0–9 are allowed in combination.";
        //            return result;
        //        }

        //        //// 🔍 Check DB for total amount bet on this combo + bet type
        //        //var comboKey = NormalizeCombo(result.Combination);
        //        //int totalSoFar = _db.BetRecords
        //        //    .Where(b => NormalizeCombo(b.Combination) == comboKey && b.BetType == result.BetType)
        //        //    .Sum(b => (int?)b.Amount) ?? 0;

        //        //if ((totalSoFar + result.Amount) > 300)
        //        //{
        //        //    result.Error = $"Total amount for {result.Combination}-{result.BetType} exceeds ₱300 (Current: ₱{totalSoFar})";
        //        //}

        //        //// 🔍 Check DB for total amount bet on this combo + bet type
        //        //var comboKey = NormalizeCombo(result.Combination);
        //        //int totalSoFar = _db.BetRecords
        //        //    .Where(b => NormalizeCombo(b.Combination) == comboKey && b.BetType == result.BetType)
        //        //    .Sum(b => (int?)b.Amount) ?? 0;

        //        //if ((totalSoFar + result.Amount) > 300)
        //        //{
        //        //    result.Error = $"Total amount for {result.Combination}-{result.BetType} exceeds ₱300 (Current: ₱{totalSoFar})";
        //        //}

        //        //_db.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Error = "System error: " + ex.Message;
        //    }

        //    return result;
        //}
        //private bool IsTrio(string combo)
        //{
        //    var digits = combo.Replace("-", "");
        //    return digits.Length == 3 && digits.Distinct().Count() == 1;
        //}
        //private string NormalizeThreeDCombo(string combo)
        //{
        //    if (string.IsNullOrWhiteSpace(combo))
        //        return string.Empty; // or throw new ArgumentException("Combination is required");

        //    var digits = combo.Replace("-", "").ToCharArray();

        //    if (digits.Length != 3)
        //        return string.Empty;

        //    Array.Sort(digits);
        //    return string.Join("-", digits);
        //}
        //#endregion

        #region 2D
        private async Task InitializeTwoDComponents(TwoDViewModel model)
        {
            // Load Draw Settings
            var drawSettings = await _drawSettings.GetDataAsync();
            var filteredDrawSettings = drawSettings
                .Where(ds => ds.DrawType.Contains("2D"))
                .ToList();

            // Populate DrawTypeOptions
            model.DrawTypeOptions = filteredDrawSettings
                .Select(ds => new SelectListItem
                {
                    Text = ds.DrawType,
                    Value = ds.DrawType
                }).ToList();

            // Get current Philippine time
            DateTime now = _dateTimeHelper.GetPhilippineTime();
            DateTime drawDateFinal;

            // Set default selected draw type if not already set
            if (model.DrawDate != DateTime.MinValue)
                drawDateFinal = model.DrawDate;
            else
                drawDateFinal = now;

            //// Suppose drawDateFinal is an existing DateTime
            //var newTime = new TimeSpan(15, 30, 0); // {Hour}:{Mins} PM
            //// Override the time part by creating a new DateTime
            //drawDateFinal = drawDateFinal.Date + newTime;

            // Find the next draw based on current time and draw settings
            var currentDraw = drawSettings
                .Where(d => drawDateFinal.TimeOfDay < d.CutOffTime)
                .OrderBy(d => d.CutOffTime)
                .FirstOrDefault();

            // If no upcoming draw is found for today, check if there are any draws at all
            if (currentDraw == null)
            {
                // No upcoming cutoff time; get the latest cutoff time available
                currentDraw = drawSettings
                    .OrderByDescending(d => d.CutOffTime)
                    .FirstOrDefault();
            }

            model.DrawDate = drawDateFinal.Date;

            model.DrawType = currentDraw?.DrawType ?? "2D 2PM Draw";

            model.DrawTime = currentDraw?.DrawTime ?? drawDateFinal.TimeOfDay;

            model.DrawCutOffTime = currentDraw?.CutOffTime ?? drawDateFinal.TimeOfDay; // Default to 30 minutes after DrawTime if not set

            model.Agents = await GetAgentsUnderIncludingSelf(0);

            var validAmounts = _twoDValidAmounts.GetDataAsync().Result.Select(o => o.Amount).ToList();

            model.ValidAmountsConcat = validAmounts.Count > 0 ? string.Join(",", validAmounts): string.Empty;

            // Determine if betting is allowed
            if (drawDateFinal.Date != now.Date)
                model.IsBetAllowed = false;
            else
            {
                // Check if the current time is before the cut-off time
                if (now.TimeOfDay < model.DrawCutOffTime)
                    model.IsBetAllowed = true;
                else
                    model.IsBetAllowed = false;
            }
        }
        [Route("Bet/2d")]
        public async Task<IActionResult> TwoD()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var model = new TwoDViewModel();

            await InitializeTwoDComponents(model);

            return View(model);
        }
        [Route("Bet/2d")]
        [HttpPost]
        public async Task<IActionResult> TwoD(TwoDViewModel model, string action)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            await InitializeTwoDComponents(model);

            if (action == "Validate")
            {
                HttpContext.Session.SetString("UpdatedValidBets", string.Empty);

                var lines = model.Entry.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                Entry currentEntry = null;
                HashSet<string> bettorNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                HashSet<string> localDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                string bettorName = string.Empty;
                model.SelectedAgentText = model.SelectedAgentText;

                var validAmounts = _twoDValidAmounts.GetDataAsync().Result.Select(o => o.Amount).ToList();
                int twoDMin = _configuration.GetValue<int>("BetSettings:TwoDMin");
                int twoDMax = _configuration.GetValue<int>("BetSettings:TwoDMax");

                foreach (var line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
                {
                    if (line.StartsWith("@"))
                    {
                        //bettorName = line.Substring(1).Trim();
                        bettorName = line.Trim();
                        currentEntry = new Entry { BettorName = bettorName };
                        model.Entries.Add(currentEntry);
                        continue;
                    }
                    else
                    {
                        if (!line.Contains("=")) // It's a header like "test" or "@test"
                        {
                            ViewBag.InvalidBettorName = "There are errors on the submitted bets. Please check for the allowed Bets format. Check that Bettor Name is prefixed with @.\r\nIncorrect raw bets given";
                            model.Entries = null;
                            return View("TwoD", model);
                        }
                    }

                    if (currentEntry == null)
                        continue;

                    // Parse and validate the bet line
                    var bet = ParseTwoDBetLine(line, validAmounts, twoDMin, twoDMax);
                    if (bet.Combination != null)
                    {
                        string normalizedCombination = bet.Combination;

                        // For Rambolito, normalize by sorting numbers
                        if (bet.Type.ToLower() == "r")
                        {
                            var numbers = bet.Combination.Split('-')
                                                         .Select(n => n.Trim())
                                                         .OrderBy(n => int.Parse(n));
                            normalizedCombination = string.Join("-", numbers);
                        }

                        string key = $"{bettorName} {normalizedCombination}={bet.Amount}{bet.Type}";

                        if (localDuplicates.Contains(key))
                        {
                            bet.Error = "Duplicate entry";
                        }
                        else
                        {
                            localDuplicates.Add(key);
                        }
                    }
                    bet.id = Guid.NewGuid(); // Assign a new ID for each bet

                    currentEntry.Bets.Add(bet);
                }


                var validEntries = new List<TwoDDto>();

                //// Loop through Entries
                //foreach (var entry in batch.Entries)
                //{
                //    // Filter valid bet lines
                //    var validBets = entry.Bets
                //        .Where(bet => string.IsNullOrWhiteSpace(bet.Error))
                //        .ToList();

                //    if (!validBets.Any())
                //        continue; // Skip entries with no valid bets

                //    foreach (var bet in validBets)
                //    {
                //        var twoDDto = new TwoDDto
                //        {
                //            id = bet.id,
                //            Bettor = entry.BettorName,
                //            UserID = model.SelectedAgentID,
                //            CreateDate = _dateTimeHelper.GetPhilippineTime(),
                //            DrawType = batch.DrawType,
                //            DrawDate = batch.DrawDate,
                //            FirstDigit = bet.FirstDigit,
                //            SecondDigit = bet.SecondDigit,
                //            Type = bet.Type,
                //            Amount = bet.Amount
                //        };

                //        entries.Add(twoDDto);
                //    }
                //}
                var twoDDtos = model.Entries
                    .SelectMany(entry => entry.Bets
                        .Where(bet => string.IsNullOrWhiteSpace(bet.Error))
                        .Select(bet => new TwoDDto
                        {
                            id = bet.id,
                            Bettor = entry.BettorName,
                            UserID = model.SelectedAgentID,
                            CreateDate = _dateTimeHelper.GetPhilippineTime(),
                            DrawType = model.DrawType,
                            DrawDate = model.DrawDate,
                            FirstDigit = bet.FirstDigit,
                            SecondDigit = bet.SecondDigit,
                            Type = bet.Type,
                            Amount = bet.Amount,
                            CreatedBy = HttpContext.Session.GetInt32("UserId") ?? 0
                        }));
                validEntries.AddRange(twoDDtos);

                var requestDto = new
                {
                    requestDto = new BulkValidateTwoDEntriesRequestDto
                    {
                        Entries = validEntries
                    }
                };

                if (!requestDto.Equals(null) && requestDto.requestDto.Entries.Count > 0)
                {
                    var client = _clientFactory.CreateClient("ApiClient");
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

                    var response = await client.PostAsJsonAsync("api/TwoDApi/BulkValidateTwoD", requestDto);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<List<BulkValidateTwoDEntriesResultDto>>();
                        if (result != null && result.Any())
                        {
                            foreach (var resultItem in result)
                            {
                                // Loop through Entries
                                foreach (var entry in model.Entries)
                                {
                                    // Filter valid bet lines
                                    var validBets = entry.Bets.Where(bet => string.IsNullOrWhiteSpace(bet.Error)).ToList();
                                    var item = validBets.FirstOrDefault(o => o.id == resultItem.id); if (item != null)
                                    {
                                        //Update bet error
                                        item.Error = resultItem.Message;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        ViewBag.Error = error;
                    }
                }
                // Serialize and store in session
                HttpContext.Session.SetString("UpdatedValidBets", JsonConvert.SerializeObject(validEntries));
            }
            else if (action == "Submit") //Submitting of Bets
            {
                HttpContext.Session.SetString("TwoDSubmitResult", string.Empty);
                //var validEntries = new List<TwoDDto>();
                // Get updated valid bets from ViewBag if available
                //var updatedValidBets = ViewBag.UpdatedValidBets as List<Entry>; // Replace Bet with your actual type
                var betsJson = HttpContext.Session.GetString("UpdatedValidBets");

                List<TwoDDto>? updatedValidBets = string.IsNullOrEmpty(betsJson)
                    ? new List<TwoDDto>()
                    : JsonConvert.DeserializeObject<List<TwoDDto>>(betsJson);

                if (updatedValidBets != null)
                {
                    var requestDto = new
                    {
                        requestDto = new BulkValidateTwoDEntriesRequestDto
                        {
                            Entries = updatedValidBets
                        }
                    };

                    //TODO: Save validEntries to database
                    if (!requestDto.Equals(null) && requestDto.requestDto.Entries.Count > 0)
                    {
                        var client = _clientFactory.CreateClient("ApiClient");
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

                        var response = await client.PostAsJsonAsync("api/TwoDApi/BulkInsertTwoD", requestDto);

                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadFromJsonAsync<BulkInsertTwoDEntriesResponseDto>();

                            if (result?.EntriesResults != null)
                            {
                                // Serialize and store in session
                                HttpContext.Session.SetString("TwoDSubmitResult", JsonConvert.SerializeObject(result?.EntriesResults));
                                
                                return RedirectToAction("2dResult", "Bet");
                            }
                        }
                        else
                        {
                            var json = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
            }
           
            return View("TwoD", model);
        }
        private BetLine ParseTwoDBetLine(string line, List<decimal> validAmounts, int twoDMin, int twoDMax)
        {
            var result = new BetLine
            {
                RawInput = line
            };

            var match = Regex.Match(line, @"^(\d{1,2})\s*([^\da-zA-Z=]+|\s+)\s*(\d{1,2})\s*=\s*(?:([rRsS])\s*(\d{2,3})|(\d{2,3})\s*([rRsS]))$");
            if (!match.Success)
            {
                result.Error = $"Invalid format: {line}";
                return result;
            }

            try
            {
                var combo1 = match.Groups[1].Value.Trim();
                var separator = match.Groups[2].Value.Trim();
                var combo2 = match.Groups[3].Value.Trim();
                var amountStr = match.Groups[5].Success ? match.Groups[5].Value : match.Groups[6].Value;//match.Groups[4].Value;
                var type = match.Groups[4].Success ? match.Groups[4].Value : match.Groups[7].Value;//match.Groups[5].Value.ToUpper();

                result.Combination = combo1 + "-" + combo2;
                result.FirstDigit = Convert.ToInt32(combo1);
                result.SecondDigit = Convert.ToInt32(combo2);
                result.Amount = Convert.ToDecimal(amountStr);
                result.Type = type.ToUpper();

                bool isValidCombo1 = int.TryParse(combo1, out int num1) && num1 >= twoDMin && num1 <= twoDMax;
                bool isValidCombo2 = int.TryParse(combo2, out int num2) && num2 >= twoDMin && num2 <= twoDMax;

                if (!isValidCombo1 || !isValidCombo2)
                {
                    result.Error = $"Only numbers from {twoDMin} to {twoDMax} are allowed.";
                    return result;
                }

                if (!decimal.TryParse(amountStr, out decimal amount))
                {
                    result.Error = $"Invalid amount: {amountStr}";
                    return result;
                }

                if (!validAmounts.Contains(amount))
                {
                    result.Error = $"Amount {amount} is not allowed.";
                    return result;
                }

                result.Amount = amount;
                // Add additional 2D-only logic if needed
            }
            catch (Exception ex)
            {
                result.Error = "System error: " + ex.Message;
            }

            return result;
        }

        [Route("Bet/LoadTwoDBetsAsync")]
        [HttpGet]
        public async Task<IActionResult> LoadTwoDBetsAsync(string drawType)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return Json(new { error = "Unauthorized" });

            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", HttpContext.Session.GetString("JWToken"));

            int? userId = HttpContext.Session.GetInt32("UserId");
            var drawDate = _dateTimeHelper.GetPhilippineTime();
            var response = await client.GetAsync(
                $"api/TwoDApi/GetBetsByUserIdDrawTypeDrawDate?userid={userId}&drawType={Uri.EscapeDataString(drawType)}&drawDate={drawDate.ToString("MM/dd/yyyy")}"
            );
            //var response = await client.PostAsJsonAsync("api/TwoDApi/GetBetsByUserIdDrawTypeDrawDate", requestDto);

            if (response.IsSuccessStatusCode)
            {
                var twoDBets = await response.Content.ReadFromJsonAsync<List<TwoDBetsDto>>();
                return Json(new { data = twoDBets });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return Json(new { error });
            }
        }

        [Route("Bet/2dResult")]
        public IActionResult TwoDResult()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            //var bulkInsertResults = TempData["TwoDSubmitResult"] as List<BulkInsertTwoDEntriesResultDto>;
            var betsJson = HttpContext.Session.GetString("TwoDSubmitResult");

            List<BulkInsertTwoDEntriesResultDto>? bulkInsertResults = string.IsNullOrEmpty(betsJson)
                ? new List<BulkInsertTwoDEntriesResultDto>()
                : JsonConvert.DeserializeObject<List<BulkInsertTwoDEntriesResultDto>>(betsJson);

            if (bulkInsertResults == null || !bulkInsertResults.Any())
            {
                return RedirectToAction("TwoD");
            }
            List<TwoDResultsViewModel> viewModelList = bulkInsertResults
                .Select(dto => new TwoDResultsViewModel
                {
                    Bettor = dto.Bettor,
                    UserID = dto.UserID,
                    CreateDate = dto.CreateDate,
                    DrawType = dto.DrawType,
                    DrawDate = dto.DrawDate,
                    id = dto.id,
                    FirstDigit = dto.FirstDigit,
                    SecondDigit = dto.SecondDigit,
                    Type = dto.Type,
                    Amount = dto.Amount,
                    IsInserted = dto.IsInserted,
                    Message = dto.Message,
                    AvailableBalance = dto.AvailableBalance
                })
                .ToList();

            return View(viewModelList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelectedBets([FromBody] BulkDeleteEntriesRequest request)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            if (request.ids == null || !request.ids.Any())
                return BadRequest("No IDs provided.");

            var client = _clientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var requestDto = new
            {
                requestDto = new BulkDeleteEntriesRequest
                {
                    ids = request.ids
                }
            };

            var response = await client.PostAsJsonAsync("api/TwoDApi/BulkDeleteTwoD", requestDto);

            if (!response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return BadRequest(json);
            }
            return Ok();
        }
        #endregion

        //    #region LP3
        //    private const int MaxRange = 31; // Change this dynamically if needed

        //    [HttpGet]
        //    public IActionResult ValidateLp3()
        //    {
        //        return View();
        //    }

        //    [HttpPost]
        //    public IActionResult ValidateLp3(string rawInput)
        //    {
        //        int maxRange = 31; // Can be from config or passed from user input
        //        var batch = LP3Parser.ParseLP3(rawInput, maxRange);

        //        // Optionally, filter and show only invalid entries
        //        var invalid = batch.Entries
        //            .SelectMany(e => e.Bets.Where(b => !string.IsNullOrEmpty(b.Error)))
        //            .ToList();

        //        // Send to view or return as JSON
        //        return View(batch);
        //    }
        //    public static class LP3Parser
        //    {
        //        private static readonly HashSet<int> ValidAmounts = new()
        //{
        //    10, 15, 20, 25, 30, 35, 40, 45, 50, 55,
        //    60, 65, 70, 75, 80, 85, 90, 95, 100,
        //    150, 200, 250, 300, 350, 400, 450, 500
        //};

        //        private static readonly Regex EntryRegex = new(
        //            @"^(?<combo>(\d{1,2})\s*-\s*(\d{1,2})\s*-\s*(\d{1,2}))\s*=\s*(?:(?<amount>\d{2,3})\s*(?<type>RB)?|(?<typeOnly>FB))$",
        //            RegexOptions.IgnoreCase | RegexOptions.Compiled
        //        );

        //        public static LottoBatch ParseLP3(string rawInput, int maxRange)
        //        {
        //            var batch = new LottoBatch();
        //            var lines = rawInput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
        //                                .Select(l => l.Trim()).ToList();

        //            LottoEntry currentEntry = null;

        //            foreach (var line in lines)
        //            {
        //                if (line.StartsWith("@"))
        //                {
        //                    currentEntry = new LottoEntry { BettorName = line };
        //                    batch.Entries.Add(currentEntry);
        //                    continue;
        //                }

        //                if (currentEntry == null)
        //                {
        //                    // Invalid - bet without bettor name
        //                    batch.Entries.Add(new LottoEntry
        //                    {
        //                        BettorName = "Unknown",
        //                        Bets = new List<BetLine>
        //                {
        //                    new BetLine
        //                    {
        //                        RawInput = line,
        //                        Error = "Bet provided before @Bettor line"
        //                    }
        //                }
        //                    });
        //                    continue;
        //                }

        //                var match = EntryRegex.Match(line);
        //                var betLine = new BetLine { RawInput = line, Bettor = currentEntry.BettorName };

        //                if (!match.Success)
        //                {
        //                    betLine.Error = "Invalid format";
        //                    currentEntry.Bets.Add(betLine);
        //                    continue;
        //                }

        //                var comboPart = match.Groups["combo"].Value;
        //                var numbers = comboPart.Split('-').Select(n => n.Trim()).ToArray();

        //                if (numbers.Length != 3)
        //                {
        //                    betLine.Error = "Combination must have exactly 3 numbers";
        //                    currentEntry.Bets.Add(betLine);
        //                    continue;
        //                }

        //                if (numbers.Any(n => !int.TryParse(n, out int num) || num < 1 || num > maxRange))
        //                {
        //                    betLine.Error = $"Numbers must be in range 1–{maxRange}";
        //                    currentEntry.Bets.Add(betLine);
        //                    continue;
        //                }

        //                var amountStr = match.Groups["amount"].Value;
        //                var type = match.Groups["type"].Value.ToUpper();
        //                var typeOnly = match.Groups["typeOnly"].Value.ToUpper();

        //                if (!string.IsNullOrEmpty(typeOnly))
        //                {
        //                    if (typeOnly != "FB")
        //                    {
        //                        betLine.Error = "Invalid bet type";
        //                    }
        //                    else
        //                    {
        //                        betLine.Amount = 0;
        //                        betLine.Type = "FB";
        //                        betLine.Combination = string.Join("-", numbers);
        //                    }
        //                }
        //                else
        //                {
        //                    if (!int.TryParse(amountStr, out int amount) || !ValidAmounts.Contains(amount))
        //                    {
        //                        betLine.Error = "Invalid amount";
        //                    }
        //                    else if (!string.IsNullOrEmpty(type) && type != "RB")
        //                    {
        //                        betLine.Error = "Invalid bet type";
        //                    }
        //                    else
        //                    {
        //                        betLine.Amount = amount;
        //                        betLine.Type = string.IsNullOrEmpty(type) ? "NORMAL" : type;
        //                        betLine.Combination = string.Join("-", numbers);
        //                    }
        //                }

        //                currentEntry.Bets.Add(betLine);
        //            }

        //            return batch;
        //        }
        //    }
        //    #endregion
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using TresDos.Application.DTOs;
using TresDos.Core.Entities;
using TresDos.Helper;

namespace TresDos.Controllers.Web
{
    public class BetController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        DateTimeHelper _dateTimeHelper = new DateTimeHelper();
        private static readonly List<int> ValidAmounts = Enumerable.Range(2, 60).Select(i => i * 5).ToList(); // 10 to 300
        public BetController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        #region Agents
        private List<SelectListItem> GetAgents()
        {
            var agents = new AgentService();
            return agents.GetAgentSelectList();
        }
        #endregion

        #region 3D
        [Route("Bet/3d")]
        public ActionResult ThreeD()
        {
            return View();
        }
        [Route("Bet/3d")]
        [HttpPost]
        public ActionResult ThreeD(string rawInput)
        {
            var batch = new LottoBatch();
            var lines = rawInput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            LottoEntry currentEntry = null;
            HashSet<string> bettorNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> localDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
            {
                if (line.StartsWith("@"))
                {
                    string bettorName = line.Substring(1).Trim();

                    // Check for duplicate bettor
                    if (bettorNames.Contains(bettorName))
                    {
                        currentEntry = new LottoEntry
                        {
                            BettorName = bettorName + " (DUPLICATE)",
                        };
                        currentEntry.Bets.Add(new BetLine
                        {
                            RawInput = line,
                            Error = $"Duplicate bettor name: {bettorName}"
                        });
                        batch.Entries.Add(currentEntry);
                        continue;
                    }

                    bettorNames.Add(bettorName);
                    currentEntry = new LottoEntry { BettorName = bettorName };
                    batch.Entries.Add(currentEntry);
                    localDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Reset for each bettor
                    continue;
                }

                if (currentEntry == null)
                    continue; // Skip lines if no bettor name yet

                var bet = ParseThreeDBetLine(line);
                string key = $"{NormalizeThreeDCombo(bet.Combination)}-{bet.BetType}";

                if (localDuplicates.Contains(key))
                {
                    bet.Error = "Duplicate entry";
                }
                else
                {
                    localDuplicates.Add(key);
                }

                currentEntry.Bets.Add(bet);
            }

            return View("ResultThreeD", batch);
        }
        private BetLine ParseThreeDBetLine(string line)
        {
            var result = new BetLine
            {
                RawInput = line // Store original format
            };

            var match = Regex.Match(line, @"^(\d{3}|\d\s*-\s*\d\s*-\s*\d|\d\s*\*\s*\d\s*\*\s*\d|\d\s+\d\s+\d)\s*=\s*(\d{2,3})([SRsr])$");
            if (!match.Success)
            {
                result.Error = $"Invalid format: {line}";
                return result;
            }
            try
            {

                var combo = match.Groups[1].Value;
                var amountStr = match.Groups[2].Value;
                var type = match.Groups[3].Value.ToUpper();

                //result.Bettor = line.Substring(1).Trim();
                result.Combination = combo.Contains("-") ? combo : string.Join("-", combo.ToCharArray());
                result.Amount = amountStr;
                result.BetType = type;

                // Normalize combination
                string comboDigits = combo.Replace(" ", "").Replace("-", "");
                if (comboDigits.Length != 3)
                {
                    result.Error = "Combination must contain exactly 3 digits.";
                    return result;
                }

                result.Combination = string.Join("-", comboDigits.ToCharArray());
                result.BetType = type;

                // Amount parsing
                if (!int.TryParse(amountStr, out int amount))
                {
                    result.Error = $"Invalid amount: {amountStr}";
                    return result;
                }

                result.Amount = amount.ToString();

                // Validate amount is in list
                if (!ValidAmounts.Contains(amount))
                {
                    result.Error = $"Amount {amount} is not allowed.";
                    return result;
                }

                // Trio check
                if (IsTrio(combo) && type == "R")
                {
                    result.Error = $"Trio combinations can only have straight bets (S): {line}";
                }

                // ✅ Check that all chars are digits 0–9
                if (!comboDigits.All(c => char.IsDigit(c) && c >= '0' && c <= '9'))
                {
                    result.Error = "Only digits 0–9 are allowed in combination.";
                    return result;
                }

                //// 🔍 Check DB for total amount bet on this combo + bet type
                //var comboKey = NormalizeCombo(result.Combination);
                //int totalSoFar = _db.BetRecords
                //    .Where(b => NormalizeCombo(b.Combination) == comboKey && b.BetType == result.BetType)
                //    .Sum(b => (int?)b.Amount) ?? 0;

                //if ((totalSoFar + result.Amount) > 300)
                //{
                //    result.Error = $"Total amount for {result.Combination}-{result.BetType} exceeds ₱300 (Current: ₱{totalSoFar})";
                //}

                //// 🔍 Check DB for total amount bet on this combo + bet type
                //var comboKey = NormalizeCombo(result.Combination);
                //int totalSoFar = _db.BetRecords
                //    .Where(b => NormalizeCombo(b.Combination) == comboKey && b.BetType == result.BetType)
                //    .Sum(b => (int?)b.Amount) ?? 0;

                //if ((totalSoFar + result.Amount) > 300)
                //{
                //    result.Error = $"Total amount for {result.Combination}-{result.BetType} exceeds ₱300 (Current: ₱{totalSoFar})";
                //}

                //_db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.Error = "System error: " + ex.Message;
            }

            return result;
        }
        private bool IsTrio(string combo)
        {
            var digits = combo.Replace("-", "");
            return digits.Length == 3 && digits.Distinct().Count() == 1;
        }
        private string NormalizeThreeDCombo(string combo)
        {
            if (string.IsNullOrWhiteSpace(combo))
                return string.Empty; // or throw new ArgumentException("Combination is required");

            var digits = combo.Replace("-", "").ToCharArray();

            if (digits.Length != 3)
                return string.Empty;

            Array.Sort(digits);
            return string.Join("-", digits);
        }
        #endregion

        #region 2D
        [Route("Bet/2d")]
        public ActionResult TwoD()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var batch = new LottoBatch();
            batch.TimeOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "2PM", Text = "2PM" },
                new SelectListItem { Value = "5PM", Text = "5PM" },
                new SelectListItem { Value = "9PM", Text = "9PM" }
            };

            // Optionally pre-select a value
            var now = _dateTimeHelper.GetPhilippineTime();
            batch.SelectedDate = now.Date;

            var twoPm = now.Date.AddHours(14);
            var fivePm = now.Date.AddHours(17);

            string selectedLabel = now <= twoPm ? "2PM" :
                                   now <= fivePm ? "5PM" : "9PM";

            batch.SelectedTime = selectedLabel;

            batch.Agents = GetAgents();
            return View(batch);
        }
        [Route("Bet/2d")]
        [HttpPost]
        public ActionResult TwoD(LottoBatch rawInput, string action)
        {
            var batch = new LottoBatch();
            batch.Agents = GetAgents();
            
            if (action == "Validate")
            { 
                var lines = rawInput.Entry.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                LottoEntry currentEntry = null;
                HashSet<string> bettorNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                HashSet<string> localDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                string bettorName = string.Empty;
                batch.SelectedAgentText = rawInput.SelectedAgentText;

                foreach (var line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
                {
                    if (line.StartsWith("@"))
                    {
                        bettorName = line.Substring(1).Trim();

                        //if (bettorNames.Contains(bettorName))
                        //{
                        //    currentEntry = new LottoEntry
                        //    {
                        //        BettorName = bettorName + " (DUPLICATE)",
                        //    };
                        //    currentEntry.Bets.Add(new BetLine
                        //    {
                        //        RawInput = line,
                        //        Error = $"Duplicate bettor name: {bettorName}"
                        //    });
                        //    batch.Entries.Add(currentEntry);
                        //    continue;
                        //}

                        //bettorNames.Add(bettorName);
                        currentEntry = new LottoEntry { BettorName = bettorName };
                        batch.Entries.Add(currentEntry);
                        //localDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        continue;
                    }
                    else
                    {
                        // Invalid - bet without bettor name
                        //if (currentEntry == null)
                        //{
                        //    currentEntry = new LottoEntry
                        //    {
                        //        BettorName = "Unknown",
                        //        Bets = new List<BetLine>
                        //        {
                        //            new BetLine
                        //            {
                        //                RawInput = line,
                        //                Error = "Bet provided before @Bettor line"
                        //            }
                        //        }
                        //    };
                        //    batch.Entries.Add(currentEntry);
                        //}
                        if (!line.Contains("=")) // It's a header like "test" or "@test"
                        {
                            ViewBag.InvalidBettorName = "There are errors on the submitted bets. Please check for the allowed Bets format. Check that Bettor Name is prefixed with @.\r\nIncorrect raw bets given";
                            batch.Entries = null;
                            return View("TwoD", batch);
                        }
                    }

                    if (currentEntry == null)
                        continue;

                    var bet = Parse2DBetLine(line);
                    //string key = $"{Normalize2DCombo(bet.Combination)}-{bet.BetType}"bet.Combination
                    //if (bet.Combination != null)
                    //{
                    //    string key = $"{bettorName} {bet.Combination}={bet.Amount}{bet.BetType}";

                    //    if (localDuplicates.Contains(key))
                    //    {
                    //        bet.Error = "Duplicate entry";
                    //    }
                    //    else
                    //    {
                    //        localDuplicates.Add(key);
                    //    }
                    //}
                    if (bet.Combination != null)
                    {
                        string normalizedCombination = bet.Combination;

                        // For Rambolito, normalize by sorting numbers
                        if (bet.BetType.ToLower() == "r")
                        {
                            var numbers = bet.Combination.Split('-')
                                                         .Select(n => n.Trim())
                                                         .OrderBy(n => int.Parse(n));
                            normalizedCombination = string.Join("-", numbers);
                        }

                        string key = $"{bettorName} {normalizedCombination}={bet.Amount}{bet.BetType}";

                        if (localDuplicates.Contains(key))
                        {
                            bet.Error = "Duplicate entry";
                        }
                        else
                        {
                            localDuplicates.Add(key);
                        }
                    }

                    currentEntry.Bets.Add(bet);
                }
            }
            return View("TwoD", batch);
        }
        private BetLine Parse2DBetLine(string line)
        {
            var result = new BetLine
            {
                RawInput = line
            };

            //var match = Regex.Match(line, @"^@[\w\s\.]+$|^(\d{1,2})\s*[-*\s]\s*(\d{1,2})\s*=\s*(\d{2,3})([sSrR])$");
            //var match = Regex.Match(line, @"^(\d{1,2})\s*([^\da-zA-Z=]+|\s+)\s*(\d{1,2})\s*=\s*(\d{2,3})([sSrR])$");
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

                result.Bettor = line.Substring(1).Trim();
                result.Combination = combo1 + "-" + combo2;
                result.Amount = amountStr;
                result.BetType = type.ToUpper();

                bool isValidCombo1 = int.TryParse(combo1, out int num1) && num1 >= 1 && num1 <= 31;
                bool isValidCombo2 = int.TryParse(combo2, out int num2) && num2 >= 1 && num2 <= 31;

                if (!isValidCombo1 || !isValidCombo2)
                {
                    result.Error = "Only numbers from 1 to 31 are allowed.";
                    return result;
                }

                if (!int.TryParse(amountStr, out int amount))
                {
                    result.Error = $"Invalid amount: {amountStr}";
                    return result;
                }

                if (!ValidAmounts.Contains(amount))
                {
                    result.Error = $"Amount {amount} is not allowed.";
                    return result;
                }

                result.Amount = amount.ToString();
                // Add additional 2D-only logic if needed
            }
            catch (Exception ex)
            {
                result.Error = "System error: " + ex.Message;
            }

            return result;
        }
        //private string Normalize2DCombo(string combo)
        //{
        //    if (string.IsNullOrWhiteSpace(combo))
        //        return string.Empty;

        //    var digits = combo.Replace(" ", "-").ToCharArray();

        //    //if (digits.Length != 2)
        //    //    return string.Empty;

        //    Array.Sort(digits);
        //    return string.Join("-", digits);
        //}
        #endregion

        #region LP3
        private const int MaxRange = 31; // Change this dynamically if needed

        [HttpGet]
        public IActionResult ValidateLp3()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ValidateLp3(string rawInput)
        {
            int maxRange = 31; // Can be from config or passed from user input
            var batch = LP3Parser.ParseLP3(rawInput, maxRange);

            // Optionally, filter and show only invalid entries
            var invalid = batch.Entries
                .SelectMany(e => e.Bets.Where(b => !string.IsNullOrEmpty(b.Error)))
                .ToList();

            // Send to view or return as JSON
            return View(batch);
        }
        public static class LP3Parser
        {
            private static readonly HashSet<int> ValidAmounts = new()
    {
        10, 15, 20, 25, 30, 35, 40, 45, 50, 55,
        60, 65, 70, 75, 80, 85, 90, 95, 100,
        150, 200, 250, 300, 350, 400, 450, 500
    };

            private static readonly Regex EntryRegex = new(
                @"^(?<combo>(\d{1,2})\s*-\s*(\d{1,2})\s*-\s*(\d{1,2}))\s*=\s*(?:(?<amount>\d{2,3})\s*(?<type>RB)?|(?<typeOnly>FB))$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled
            );

            public static LottoBatch ParseLP3(string rawInput, int maxRange)
            {
                var batch = new LottoBatch();
                var lines = rawInput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(l => l.Trim()).ToList();

                LottoEntry currentEntry = null;

                foreach (var line in lines)
                {
                    if (line.StartsWith("@"))
                    {
                        currentEntry = new LottoEntry { BettorName = line };
                        batch.Entries.Add(currentEntry);
                        continue;
                    }

                    if (currentEntry == null)
                    {
                        // Invalid - bet without bettor name
                        batch.Entries.Add(new LottoEntry
                        {
                            BettorName = "Unknown",
                            Bets = new List<BetLine>
                    {
                        new BetLine
                        {
                            RawInput = line,
                            Error = "Bet provided before @Bettor line"
                        }
                    }
                        });
                        continue;
                    }

                    var match = EntryRegex.Match(line);
                    var betLine = new BetLine { RawInput = line, Bettor = currentEntry.BettorName };

                    if (!match.Success)
                    {
                        betLine.Error = "Invalid format";
                        currentEntry.Bets.Add(betLine);
                        continue;
                    }

                    var comboPart = match.Groups["combo"].Value;
                    var numbers = comboPart.Split('-').Select(n => n.Trim()).ToArray();

                    if (numbers.Length != 3)
                    {
                        betLine.Error = "Combination must have exactly 3 numbers";
                        currentEntry.Bets.Add(betLine);
                        continue;
                    }

                    if (numbers.Any(n => !int.TryParse(n, out int num) || num < 1 || num > maxRange))
                    {
                        betLine.Error = $"Numbers must be in range 1–{maxRange}";
                        currentEntry.Bets.Add(betLine);
                        continue;
                    }

                    var amountStr = match.Groups["amount"].Value;
                    var type = match.Groups["type"].Value.ToUpper();
                    var typeOnly = match.Groups["typeOnly"].Value.ToUpper();

                    if (!string.IsNullOrEmpty(typeOnly))
                    {
                        if (typeOnly != "FB")
                        {
                            betLine.Error = "Invalid bet type";
                        }
                        else
                        {
                            betLine.Amount = "0";
                            betLine.BetType = "FB";
                            betLine.Combination = string.Join("-", numbers);
                        }
                    }
                    else
                    {
                        if (!int.TryParse(amountStr, out int amount) || !ValidAmounts.Contains(amount))
                        {
                            betLine.Error = "Invalid amount";
                        }
                        else if (!string.IsNullOrEmpty(type) && type != "RB")
                        {
                            betLine.Error = "Invalid bet type";
                        }
                        else
                        {
                            betLine.Amount = amount.ToString();
                            betLine.BetType = string.IsNullOrEmpty(type) ? "NORMAL" : type;
                            betLine.Combination = string.Join("-", numbers);
                        }
                    }

                    currentEntry.Bets.Add(betLine);
                }

                return batch;
            }
        }
        #endregion
    }
}

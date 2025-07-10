using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using TresDos.Models;

namespace TresDos.Controllers
{
    public class BetController : Controller
    {
        private static readonly List<int> ValidAmounts = Enumerable.Range(2, 60).Select(i => i * 5).ToList(); // 10 to 300

        public ActionResult ThreeD()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Submit(string rawInput)
        {
            //var model = new LottoEntry();
            //var lines = rawInput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            //var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Track combo+type

            //foreach (var line in lines)
            //{
            //    //if (line.StartsWith("@"))
            //    //{
            //    //    model.BettorName = line.Substring(1).Trim();
            //    //    continue;
            //    //}

            //    var bet = ParseBetLine(line.Trim());
            //    var key = $"{NormalizeCombo(bet.Combination)}-{bet.BetType}";

            //    if (seen.Contains(key))
            //    {
            //        bet.Error = "Duplicate entry";
            //    }
            //    else
            //    {
            //        seen.Add(key);
            //    }

            //    model.Bets.Add(bet);
            //}

            //return View("Result", model);

            //var batch = new LottoBatch();
            //var lines = rawInput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            //LottoEntry currentEntry = null;
            //HashSet<string> localDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            //foreach (var line in lines.Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)))
            //{
            //    if (line.StartsWith("@"))
            //    {
            //        // New bettor
            //        currentEntry = new LottoEntry { BettorName = line.Substring(1).Trim() };
            //        batch.Entries.Add(currentEntry);
            //        localDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase); // Reset for new bettor
            //        continue;
            //    }

            //    if (currentEntry == null)
            //        continue; // Skip invalid line if no bettor is defined yet

            //    var bet = ParseBetLine(line);
            //    string key = $"{NormalizeCombo(bet.Combination)}-{bet.BetType}";

            //    if (localDuplicates.Contains(key))
            //    {
            //        bet.Error = "Duplicate entry";
            //    }
            //    else
            //    {
            //        localDuplicates.Add(key);
            //    }

            //    currentEntry.Bets.Add(bet);
            //}

            //return View("Result", batch);

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

                var bet = ParseBetLine(line);
                string key = $"{NormalizeCombo(bet.Combination)}-{bet.BetType}";

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

            return View("Result", batch);
        }

        private BetLine ParseBetLine(string line)
        {
            var result = new BetLine
            {
                RawInput = line // Store original format
            };

            // Validate format: e.g. 1-2-3=50S or 777=10R
            //var match = Regex.Match(line, @"^(\d{1}-\d{1}-\d{1}|\d{3})=(\d+)([SR])$", RegexOptions.IgnoreCase);
            //var match = Regex.Match(line, @"^(\d{1}-\d{1}-\d{1}|\d{3})=(\d{2,3})([SR])$", RegexOptions.IgnoreCase);
            //var match = Regex.Match(line, @"^(\d{3}|\d-\d-\d)=(\d{2,3})([SR])$", RegexOptions.IgnoreCase);
            var match = Regex.Match(line, @"^(\d{3}|\d-\d-\d)=(\d{2,3})([SRsr])$");
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

                result.Bettor = line.Substring(1).Trim();
                result.Combination = combo.Contains("-") ? combo : string.Join("-", combo.ToCharArray());
                result.Amount = amountStr;
                result.BetType = type;

                // Normalize combination
                string comboDigits = combo.Replace("-", "");
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
            //string cleaned = combo.Replace("-", "");
            //return cleaned.Length == 3 && cleaned[0] == cleaned[1] && cleaned[1] == cleaned[2];
            var digits = combo.Replace("-", "");
            return digits.Length == 3 && digits.Distinct().Count() == 1;
        }
        private string NormalizeCombo(string combo)
        {
            // Remove dashes, convert to char[], sort, and reformat with dashes
            //var digits = combo.Replace("-", "").ToCharArray();
            //return string.Join("-", digits);

            if (string.IsNullOrWhiteSpace(combo))
                return string.Empty; // or throw new ArgumentException("Combination is required");

            var digits = combo.Replace("-", "").ToCharArray();

            if (digits.Length != 3)
                return string.Empty;

            Array.Sort(digits);
            return string.Join("-", digits);
        }
    }
}

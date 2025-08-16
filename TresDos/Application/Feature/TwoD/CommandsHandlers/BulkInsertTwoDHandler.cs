using AutoMapper;
using MediatR;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;
using TresDos.Application.Feature.TwoD.Commands;
using TresDos.Application.DTOs.BetDto;

namespace TresDos.Application.Feature.TwoD.Handlers
{
    public class BulkInsertTwoDHandler : IRequestHandler<BulkInsertTwoDCommand, (List<tb_TwoD>, List<BulkInsertTwoDEntriesProcessingResultDto>)>
    {
        private readonly ITwoDRepository _repo;
        private readonly IConfiguration _configuration;
        public BulkInsertTwoDHandler(ITwoDRepository repo, IConfiguration configuration)
        {
            _repo = repo;
            _configuration = configuration;
        }

        public async Task<(List<tb_TwoD>, List<BulkInsertTwoDEntriesProcessingResultDto>)> Handle(BulkInsertTwoDCommand request, CancellationToken cancellationToken)
        {
            var inserted = new List<tb_TwoD>();
            var results = new List<BulkInsertTwoDEntriesProcessingResultDto>();

            decimal maxLimitPerCombo = _configuration.GetValue<decimal>("BetSettings:TwoDMaxBet");

            var runningTotals = new Dictionary<string, decimal>();

            foreach (var dto in request.requestDto.Entries)
            {
                string validationKey = GetValidationKey(dto);

                // 1. Check if this exact entry already exists (including bettor)
                bool exists = await _repo.EntryExistsAsync(
                    dto.Bettor, 
                    dto.FirstDigit, 
                    dto.SecondDigit, 
                    dto.Amount, 
                    dto.Type,
                    dto.DrawType,
                    dto.DrawDate
                    );
                if (exists)
                {
                    results.Add(Failed(dto, "Duplicate entry already exists", GetAvailable(runningTotals, validationKey, maxLimitPerCombo)));
                    continue;
                }

                // 2. Load running total from DB if first time seeing this combo
                if (!runningTotals.ContainsKey(validationKey))
                {
                    decimal existing = await _repo.GetCurrentTotalAsync(
                        dto.Type, 
                        dto.FirstDigit, 
                        dto.SecondDigit,
                        dto.DrawType,
                        dto.DrawDate
                        );
                    runningTotals[validationKey] = existing;
                }

                decimal currentTotal = runningTotals[validationKey];
                decimal available = maxLimitPerCombo - currentTotal;

                // 3. Validate against available amount for that combo
                if (dto.Amount <= available)
                {
                    var entry = new tb_TwoD
                    {
                        id = dto.id,
                        Bettor = dto.Bettor,
                        FirstDigit = dto.FirstDigit,
                        SecondDigit = dto.SecondDigit,
                        Type = dto.Type,
                        Amount = dto.Amount,
                        UserID = dto.UserID,
                        CreateDate = dto.CreateDate,
                        DrawType = dto.DrawType,
                        DrawDate = dto.DrawDate,
                    };

                    inserted.Add(entry);
                    runningTotals[validationKey] += dto.Amount;

                    results.Add(new BulkInsertTwoDEntriesProcessingResultDto
                    {
                        id = dto.id,
                        Bettor = dto.Bettor,
                        FirstDigit = dto.FirstDigit,
                        SecondDigit = dto.SecondDigit,
                        Type = dto.Type,
                        Amount = dto.Amount,
                        UserID = dto.UserID,
                        CreateDate = dto.CreateDate,
                        DrawType = dto.DrawType,
                        DrawDate = dto.DrawDate,
                        IsInserted = true,
                        Message = "Inserted",
                        AvailableBalance = maxLimitPerCombo - runningTotals[validationKey]
                    });
                }
                else
                {
                    results.Add(Failed(dto, $"Exceeds available balance of {available} for this combination", available));
                }
            }

            if (inserted.Any())
                await _repo.AddEntriesAsync(inserted);

            return (inserted, results);
        }

        private static string GetValidationKey(TwoDDto dto) =>
            $"{dto.FirstDigit}_{dto.SecondDigit}_{dto.Type}";

        private static BulkInsertTwoDEntriesProcessingResultDto Failed(TwoDDto dto, string message, decimal balance) =>
            new BulkInsertTwoDEntriesProcessingResultDto
            {
                id = dto.id,
                Bettor = dto.Bettor,
                FirstDigit = dto.FirstDigit,
                SecondDigit = dto.SecondDigit,
                Type = dto.Type,
                Amount = dto.Amount,
                UserID = dto.UserID,
                CreateDate = dto.CreateDate,
                DrawType = dto.DrawType,
                DrawDate = dto.DrawDate,
                // No need to set id here, it will be generated by the repository
                IsInserted = false,
                Message = message,
                AvailableBalance = balance
            };

        private static decimal GetAvailable(Dictionary<string, decimal> totals, string key, decimal max) =>
            max - (totals.ContainsKey(key) ? totals[key] : 0);
    }
}

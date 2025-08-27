using Microsoft.EntityFrameworkCore;
using TresDos.Application.DTOs.BetDto;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;
using TresDos.Infrastructure.Data;
using EFCore.BulkExtensions;

namespace TresDos.Infrastructure.Repositories
{
    public class TwoDRepository : ITwoDRepository
    {
        private readonly AppDbContext _context;

        public TwoDRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<tb_TwoD>> GetAllAsync() => await _context.tb_TwoD.ToListAsync();

        public async Task<tb_TwoD?> GetByIdAsync(int id) => await _context.tb_TwoD.FindAsync(id);

        public async Task AddAsync(tb_TwoD entry)
        {
            _context.tb_TwoD.Add(entry);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(tb_TwoD entry)
        {
            _context.tb_TwoD.Update(entry);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entry = await _context.tb_TwoD.FindAsync(id);
            if (entry != null)
            {
                _context.tb_TwoD.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetCurrentTotalAsync(
            string typeCode,
            int firstDigit,
            int secondDigit,
            string drawType,
            DateTime drawDate)
        {
            // This method combines the logic for both RAMBLE and STRAIGHT types
            decimal result;

            if (typeCode == "R")
            {
                 result = await _context.tb_TwoD
                    .Where(e => e.Type == typeCode &&
                                e.DrawType == drawType &&
                                e.DrawDate.Date == drawDate.Date &&
                               ((e.FirstDigit == firstDigit && e.SecondDigit == secondDigit) ||
                                (e.FirstDigit == secondDigit && e.SecondDigit == firstDigit)))
                    .SumAsync(e => e.Amount);
            }
            else
            {
                result= await _context.tb_TwoD
                        .Where(e => e.Type == typeCode &&
                                    e.DrawType == drawType &&
                                    e.DrawDate.Date == drawDate.Date &&
                                   ((e.FirstDigit == firstDigit && e.SecondDigit == secondDigit) ||
                                    (e.FirstDigit == secondDigit && e.SecondDigit == secondDigit)))
                        .SumAsync(e => e.Amount);
            }
            return result;
        }

        public async Task AddEntriesAsync(IEnumerable<tb_TwoD> entries)
        {
            await _context.tb_TwoD.AddRangeAsync(entries);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> EntryExistsAsync(
            string bettor,
            int firstDigit,
            int secondDigit,
            decimal amount,
            string type,
            string drawType,
            DateTime drawDate)
        {
            return await _context.tb_TwoD.AnyAsync(e =>
                e.Bettor == bettor &&
                e.FirstDigit == firstDigit &&
                e.SecondDigit == secondDigit &&
                e.Amount == amount &&
                e.Type == type &&
                e.DrawType == drawType &&
                e.DrawDate.Date == drawDate.Date
            );
        }

        public async Task<List<TwoDBetsDto>?> GetBetsByUserIdDrawTypeDrawDate(int userId, string drawType, DateTime drawDate)
        {
            var bets = await (
                 from bet in _context.tb_TwoD
                 join agent in _context.Users
                     on bet.UserID equals agent.Id
                 join creator in _context.Users
                     on bet.UserID equals creator.Id
                 where bet.UserID == userId &&
                       bet.DrawType == drawType &&
                       bet.DrawDate.Date == drawDate.Date
                 orderby bet.CreateDate descending
                 select new TwoDBetsDto
                 {
                     id = bet.id,
                     Bettor = bet.Bettor,
                     FirstDigit = bet.FirstDigit,
                     SecondDigit = bet.SecondDigit,
                     Type = bet.Type,
                     Amount = bet.Amount,
                     DrawType = bet.DrawType,
                     DrawDate = bet.DrawDate,
                     Creator = $"{creator.FirstName} {creator.MiddleName} {creator.LastName}",
                     AgentName = $"{creator.FirstName} {creator.MiddleName} {creator.LastName}",
                     CreateDate = bet.CreateDate
                 }).ToListAsync();

            return bets;
        }
        public async Task RemoveEntriesAsync(BulkDeleteEntriesRequest Guids)
        {
            //var betsToDelete = await _context.tb_TwoD
            //    .Where(u => Guids.ids.Contains(u.id))
            //    .ToListAsync();

            //await _context.BulkDeleteAsync(betsToDelete);

            if (Guids?.ids == null || !Guids.ids.Any())
                return;

            var entities = await _context.tb_TwoD
                .Where(x => Guids.ids.Contains(x.id))
                .ToListAsync();

            if (entities.Any())
            {
                _context.tb_TwoD.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
        }
    }
}
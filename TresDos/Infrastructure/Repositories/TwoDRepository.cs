using Microsoft.EntityFrameworkCore;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;
using TresDos.Infrastructure.Data;

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

        //public async Task<decimal> GetRAMBLECurrentTotalAsync(
        //    string typeCode,
        //    int firstDigit,
        //    int secondDigit)
        //{
        //    return await _context.tb_TwoD
        //            .Where(e => e.Type == typeCode &&
        //                       ((e.FirstDigit == firstDigit && e.SecondDigit == secondDigit) ||
        //                        (e.FirstDigit == secondDigit && e.SecondDigit == secondDigit)))
        //            .SumAsync(e => e.Amount);
        //}
        //public async Task<decimal> GetSTRAIGHTCurrentTotalAsync(
        //    string typeCode,
        //    int firstDigit,
        //    int secondDigit)
        //{
        //    return await _context.tb_TwoD
        //           .Where(e => e.Type == typeCode &&
        //                     e.FirstDigit == firstDigit && e.SecondDigit == secondDigit)
        //           .SumAsync(e => e.Amount);
        //}

        public async Task<decimal> GetCurrentTotalAsync(
            string typeCode,
            int firstDigit,
            int secondDigit)
        {
            // This method combines the logic for both RAMBLE and STRAIGHT types
            if (typeCode == "R")
            {
                return await _context.tb_TwoD
                    .Where(e => e.Type == typeCode &&
                               ((e.FirstDigit == firstDigit && e.SecondDigit == secondDigit) ||
                                (e.FirstDigit == secondDigit && e.SecondDigit == firstDigit)))
                    .SumAsync(e => e.Amount);
            }
            else
            {
                return await _context.tb_TwoD
                        .Where(e => e.Type == typeCode &&
                                   ((e.FirstDigit == firstDigit && e.SecondDigit == secondDigit) ||
                                    (e.FirstDigit == secondDigit && e.SecondDigit == secondDigit)))
                        .SumAsync(e => e.Amount);
            }
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
            string type)
        {
            return await _context.tb_TwoD.AnyAsync(e =>
                e.Bettor == bettor &&
                e.FirstDigit == firstDigit &&
                e.SecondDigit == secondDigit &&
                e.Amount == amount &&
                e.Type == type
            );
        }
    }
}
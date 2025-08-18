using Microsoft.EntityFrameworkCore;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;
using TresDos.Infrastructure.Data;

namespace TresDos.Infrastructure.Repositories
{
    public class TwoDValidAmountsRepository : ITwoDValidAmountsRepository
    {
        private readonly AppDbContext _context;

        public TwoDValidAmountsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ltb_twoDValidAmounts>> GetAllAsync() => await _context.ltb_twoDValidAmounts.ToListAsync();

        public async Task<ltb_twoDValidAmounts?> GetByIdAsync(int id) => await _context.ltb_twoDValidAmounts.FindAsync(id);
        public async Task<ltb_twoDValidAmounts?> GetByTypeAsync(string type) => await _context.ltb_twoDValidAmounts.FindAsync(type);

        public async Task AddAsync(ltb_twoDValidAmounts setting)
        {
            _context.ltb_twoDValidAmounts.Add(setting);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ltb_twoDValidAmounts setting)
        {
            _context.ltb_twoDValidAmounts.Update(setting);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var setting = await _context.ltb_twoDValidAmounts.FindAsync(id);
            if (setting != null)
            {
                _context.ltb_twoDValidAmounts.Remove(setting);
                await _context.SaveChangesAsync();
            }
        }
    }
}
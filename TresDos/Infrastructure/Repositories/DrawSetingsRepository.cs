using Microsoft.EntityFrameworkCore;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;
using TresDos.Infrastructure.Data;

namespace TresDos.Infrastructure.Repositories
{
    public class DrawSetingsRepository : IDrawSetingsRepository
    {
        private readonly AppDbContext _context;

        public DrawSetingsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ltb_DrawSettings>> GetAllAsync() => await _context.ltb_DrawSettings.ToListAsync();

        public async Task<ltb_DrawSettings?> GetByIdAsync(int id) => await _context.ltb_DrawSettings.FindAsync(id);
        public async Task<ltb_DrawSettings?> GetByTypeAsync(string type) => await _context.ltb_DrawSettings.FindAsync(type);

        public async Task AddAsync(ltb_DrawSettings setting)
        {
            _context.ltb_DrawSettings.Add(setting);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ltb_DrawSettings setting)
        {
            _context.ltb_DrawSettings.Update(setting);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var setting = await _context.ltb_DrawSettings.FindAsync(id);
            if (setting != null)
            {
                _context.ltb_DrawSettings.Remove(setting);
                await _context.SaveChangesAsync();
            }
        }
    }
}
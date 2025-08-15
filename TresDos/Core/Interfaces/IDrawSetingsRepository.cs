using TresDos.Core.Entities;

namespace TresDos.Core.Interfaces
{
    public interface IDrawSetingsRepository
    {
        Task<IEnumerable<ltb_DrawSettings>> GetAllAsync();
        Task<ltb_DrawSettings?> GetByIdAsync(int id);
        Task<ltb_DrawSettings?> GetByTypeAsync(string type);
        Task AddAsync(ltb_DrawSettings setting);
        Task UpdateAsync(ltb_DrawSettings setting);
        Task DeleteAsync(int id);
    }
}

using TresDos.Core.Entities;

namespace TresDos.Core.Interfaces
{
    public interface ITwoDValidAmountsRepository
    {
        Task<IEnumerable<ltb_twoDValidAmounts>> GetAllAsync();
        Task<ltb_twoDValidAmounts?> GetByIdAsync(int id);
        Task<ltb_twoDValidAmounts?> GetByTypeAsync(string type);
        Task AddAsync(ltb_twoDValidAmounts setting);
        Task UpdateAsync(ltb_twoDValidAmounts setting);
        Task DeleteAsync(int id);
    }
}

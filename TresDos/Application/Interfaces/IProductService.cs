using TresDos.Application.DTOs.ProductDto;

namespace TresDos.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<TwoDDto>> GetAllAsync();
        Task<TwoDDto?> GetByIdAsync(Guid id);
        Task AddAsync(TwoDDto dto);
        Task UpdateAsync(Guid id, TwoDDto dto);
        Task DeleteAsync(Guid id);
    }
}

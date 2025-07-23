using TresDos.Application.DTOs.ProductDto;

namespace TresDos.Application.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto?> GetByIdAsync(Guid id);
        Task AddAsync(ProductDto dto);
        Task UpdateAsync(Guid id, ProductDto dto);
        Task DeleteAsync(Guid id);
    }
}

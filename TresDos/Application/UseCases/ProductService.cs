using TresDos.Application.DTOs.ProductDto;
using TresDos.Application.Interfaces;
using TresDos.Core.Entities;
using TresDos.Core.Interfaces;

namespace TresDos.Application.UseCases
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repo, ILogger<ProductService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _repo.GetAllAsync();
            return products.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price });
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = await _repo.GetByIdAsync(id);
            return product == null ? null : new ProductDto { Id = product.Id, Name = product.Name, Price = product.Price };
        }

        public async Task AddAsync(ProductDto dto)
        {
            _logger.LogInformation("Creating new product: {@Dto}", dto);
            var product = new Product { Name = dto.Name, Price = dto.Price };
            await _repo.AddAsync(product);
        }

        public async Task UpdateAsync(Guid id, ProductDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return;

            existing.Name = dto.Name;
            existing.Price = dto.Price;
            await _repo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(Guid id) => await _repo.DeleteAsync(id);
    }
}

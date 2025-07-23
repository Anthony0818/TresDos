using Microsoft.AspNetCore.Mvc;
using TresDos.Application.DTOs.ProductDto;
using TresDos.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ProductsApiController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsApiController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var product = await _service.GetByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductDto dto)
    {
        await _service.AddAsync(dto);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ProductDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
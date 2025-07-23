using MediatR;
using TresDos.Application.DTOs.ProductDto;

namespace TresDos.Application.Feature.Products.QueriesHandlers
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
}

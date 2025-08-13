using MediatR;
using TresDos.Application.DTOs.ProductDto;

namespace TresDos.Application.Feature.Products.Queries
{
    public record GetProductByIdQuery(int Id) : IRequest<TwoDDto>;
}

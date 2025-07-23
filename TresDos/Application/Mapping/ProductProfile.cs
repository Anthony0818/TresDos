using AutoMapper;
using TresDos.Application.Feature.Products.Commands;
using TresDos.Core.Entities;

namespace TresDos.Application.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductCommand, Product>();
        }
    }
}

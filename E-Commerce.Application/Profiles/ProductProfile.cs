using AutoMapper;
using E_Commerce.Application.DTOS;
using E_Commerce.Domain.Entities.Product;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Profiles
{
    public class ProductProfile:Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(s => s.ProductBrand, p => p.MapFrom(s => s.ProductBrand.Name))
                .ForMember(s => s.ProductType, p => p.MapFrom(s => s.ProductType.Name))
                .ForMember(s => s.PictureUrl, o => o.MapFrom<PictureUrlResolver>());
            CreateMap<ProductBrand, BrandDto>();
            CreateMap<ProductType, TypeDto>();

        }
    }
}

using AutoMapper;
using E_Commerce.Application.DTOS;
using E_Commerce.Domain.Entities.Product;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Profiles
{
    public class PictureUrlResolver(IOptions<UrlSetting> options) : IValueResolver<Product, ProductDto, string?>
    {
        private readonly UrlSetting _urlSettings = options.Value;
        public string? Resolve(Product source, ProductDto destination, string? destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.PictureUrl))
                return null;
            var baseUrl = _urlSettings.BaseUrl.TrimEnd('/');
            var path = source.PictureUrl.TrimStart('/');
            return $"{baseUrl}/Files/{path}";
        }
    }

    public class UrlSetting
    {
        public string BaseUrl { get; set; } = string.Empty;
    }
}

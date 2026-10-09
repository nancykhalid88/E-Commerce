using AutoMapper;
using AutoMapper.Execution;
using E_Commerce.Application.DTOS.Order;
using E_Commerce.Domain.Entities.Orders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Profiles
{
    public class OrderItemPictureUrlResolver(IOptions<UrlSetting> options) : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly UrlSetting _urlSetting = options.Value;
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.Product.PictureUrl))
                return string.Empty;

            return $"{_urlSetting.BaseUrl}{source.Product.PictureUrl}";
        }
    }
}

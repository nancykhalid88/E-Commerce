using E_Commerce.Application.Common;
using E_Commerce.Application.DTOS.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IBasketService
    {
        Task<Result<BasketDto>> GetBasketAsync(string Id, CancellationToken ct = default);
        Task<Result<BasketDto>> CreateOrUpdateBasket(BasketDto basket, CancellationToken ct = default);
        Task<Result<bool>> DeleteTaskAync(string Id, CancellationToken ct=default);
    }
}

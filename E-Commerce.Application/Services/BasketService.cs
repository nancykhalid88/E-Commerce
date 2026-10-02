using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Basket;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class BasketService(IBasketRepository basketRepository ,IMapper _mapper) : IBasketService
    {
        public async Task<Result<BasketDto>> CreateOrUpdateBasket(BasketDto basket, CancellationToken ct = default)
        {
            var CustomerBasket=_mapper.Map<CustomerBasket>(basket);
            var BasketResult=await basketRepository.CreateOrUpdateBasketAsync(CustomerBasket, ct: ct);
            return BasketResult != null ? Result<BasketDto>.Ok(_mapper.Map<BasketDto>(BasketResult)) :
                Result<BasketDto>.Fail(Error.Failure("Cannot Create Or Update Basket"));
        }

        public async Task<Result<bool>> DeleteTaskAync(string Id, CancellationToken ct = default)
        {
            var result=await basketRepository.DeleteBasketAsync(Id, ct);
            return result ? Result<bool>.Ok(true) : Result<bool>.Fail(Error.Failure("Can Not Delete Basket"));
        }

        public async Task<Result<BasketDto>> GetBasketAsync(string Id, CancellationToken ct = default)
        {
            var basket = await basketRepository.GetBasketAsync(Id, ct);
            if (basket == null)
                return Result<BasketDto>.Fail(Error.Failure("Basket Not Found"));
            return _mapper.Map<BasketDto>(basket);
        }
    }
}

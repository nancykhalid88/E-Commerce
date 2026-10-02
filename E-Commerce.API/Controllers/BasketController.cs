using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Basket;
using E_Commerce.Domain.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class BasketController(IBasketService basketService) : ApiBaseController
    {
        #region Get
        [HttpGet("{Id}")]
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BasketDto>> GetBasket(string Id, CancellationToken ct = default)
        {
            var basket = await basketService.GetBasketAsync(Id, ct);
            return ToActionResult(basket);
        }
        #endregion
        #region CreateOrUpdate 
        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateOrUpdateBasket(BasketDto basketDto, CancellationToken ct)
        {
            var saved = await basketService.CreateOrUpdateBasket(basketDto, ct);
            return ToActionResult(saved);
        }
        #endregion
        #region Delete
        [HttpDelete("{Id}")]
        public async Task<ActionResult<bool>>DeleteBasket(string Id,CancellationToken ct)
        {
            var result = await basketService.DeleteTaskAync(Id, ct);
            return ToActionResult(result);
        }
        #endregion
    } 
}

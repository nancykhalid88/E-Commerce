using E_Commerce.Application.DTOS.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class OrdersController : ApiBaseController
    {
        #region Create Order
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(OrderToReturnDto),StatusCodes.Status200OK)]
        public async Task<ActionResult<OrderToReturnDto>>CreateOrder()
        #endregion
    }
}

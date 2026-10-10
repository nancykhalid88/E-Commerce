using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Basket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace E_Commerce.API.Controllers
{
    public class PaymentController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IOptions<PaymentGatewaySettings> _options;

        public PaymentController(IPaymentService paymentService,IOptions<PaymentGatewaySettings> options)
        {
            _paymentService = paymentService;
            _options = options;
        }
        [Authorize]
        [HttpPost("{basketId}")]
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<BasketDto>> CreateOrUpdatePaymentIntent(string basketId, CancellationToken ct)
        => ToActionResult(await _paymentService.CreateOrUpdatePaymentIntentAsync(basketId, ct));
    }
}

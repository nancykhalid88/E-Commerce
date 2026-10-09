using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Basket;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Product;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGatway;
        private readonly PaymentGatewaySettings _stripeSettings;
        private readonly IMapper _mapper;

        public PaymentService(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IPaymentGateway paymentGatway,IOptions<PaymentGatewaySettings>stripeSettings,IMapper mapper)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentGatway = paymentGatway;
            _stripeSettings = stripeSettings.Value;
            _mapper = mapper;
        }
        public async Task<Result<BasketDto>> CreateOrUpdatePaymentIntentAsync(string basketId, CancellationToken ct = default)
        {
            var basket = await _basketRepository.GetBasketAsync(basketId, ct);
            if (basket == null)
                return Result<BasketDto>.Fail(Error.NotFound("Basket Not Found", $"Basket With Id{basketId}Is Not Found"));
            if (basket.Items.Count == 0)
                return Result<BasketDto>.Fail(Error.Validation("Basket Is Empty", $"Can Not Create Order With Basket With Id:{basketId}"));
            var productRepo = _unitOfWork.GetRepository<Product, int>();
            var productIds = basket.Items.Select(i => i.Id).ToHashSet();
            var products = (await productRepo.GetAllAsync(new ProductWithIdSpecifications(productIds))).ToDictionary(x => x.Id);
            foreach (var item in basket.Items)
            {
                if (!products.TryGetValue(item.Id, out var product))
                    return Result<BasketDto>.Fail(Error.NotFound("Product Not Found", $"Product With Id{item.Id} Is Not Found"));
                item.Price = product.Price;
            }
            var deliveryRepo = _unitOfWork.GetRepository<DeliveryMethod, int>();
            var deliveryMethod = await deliveryRepo.GetByIdAsync(basket.DeliveryMethodId.Value, ct);
            if (deliveryMethod == null)
                return Result<BasketDto>.Fail(Error.NotFound("Delivery Method Not Found", $"DeliveryMethod With Id: {basket.DeliveryMethodId.Value} Is Not Found"));

            basket.ShippingPrice = deliveryMethod.Cost;
            var subtotal = basket.Items.Sum(i => i.Quantity * i.Price);
            var amount = (long)Math.Round((subtotal + deliveryMethod.Cost) + 100m);

            if (!string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                await _paymentGatway.UpadatePaymentIntentAsync(basket.PaymentIntentId, amount, ct);
            }
            else
            {
                var result = await _paymentGatway.CreatePaymentIntentAsync(amount, _stripeSettings.DefaultCurrency, ct);
                basket.PaymentIntentId = result.PaymentIntentId;
                basket.ClientSecret = result.ClientSecret;
            }
            await _basketRepository.CreateOrUpdateBasketAsync(basket, ct: ct);
            return _mapper.Map<BasketDto>(basket);
        }

        public async Task PaymentFailed(string paymentIntentId)
        {
            var OrderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var order = await OrderRepo.GetByIdAsync(new PaymentIntentSpec(paymentIntentId));
            if (order == null)
                return;
            order.Status = OrderStatus.PaymentFailed;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PaymentSucceeded(string paymentIntentId)
        {
            var OrderRepo = _unitOfWork.GetRepository<Order, Guid>();
            var order = await OrderRepo.GetByIdAsync(new PaymentIntentSpec(paymentIntentId));
            if (order == null)
                return;
            order.Status= OrderStatus.PaymentReceived;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}

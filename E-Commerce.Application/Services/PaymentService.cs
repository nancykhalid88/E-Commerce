using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Basket;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
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
        private readonly IOptions<PaymentGatewaySettings> _stripeSettings;
        private readonly IMapper _mapper;

        public PaymentService(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IPaymentGateway paymentGatway,IOptions<PaymentGatewaySettings>stripeSettings,IMapper mapper)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentGatway = paymentGatway;
            _stripeSettings = stripeSettings;
            _mapper = mapper;
        }
        public Task<Result<BasketDto>> CreateOrUpdatePaymentIntentAsync(string basketId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
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

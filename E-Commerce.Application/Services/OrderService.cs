using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOS.Basket;
using E_Commerce.Application.DTOS.Order;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class OrderService(IMapper mapper,IUnitOfWork unitOfWork,IBasketRepository basketRepository) : IOrderService
    {
        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct)
        {
            var basket = await basketRepository.GetBasketAsync(orderDto.BasketId);
            if (basket == null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Basket Not Found", $"Basket With Id{orderDto.BasketId} Not Found "));
            if (basket.Items.Count==0)
                return Result<OrderToReturnDto>.Fail(Error.Validation("Basket Is Empty", $"Cannot Create Order with Basket With Id {orderDto.BasketId}."));
            var orderRepo = unitOfWork.GetRepository<Order, Guid>();
            var productRepo = unitOfWork.GetRepository<Product, int>();

            var productIds = basket.Items.Select(p => p.Id).ToHashSet();
            var products = await productRepo.GetAllAsync(
    new ProductWithIdSpecifications(productIds));

            var productsById = products.ToDictionary(p => p.Id);

            var orderItems = new List<OrderItem>(basket.Items.Count);

            foreach (var item in basket.Items)
            {
                if (!productsById.TryGetValue(item.Id, out var product))
                {
                    return Result<OrderToReturnDto>.Fail(
                        Error.NotFound(
                            "Product Not Found",
                            $"Product with Id: {item.Id} was not found"));
                }

                orderItems.Add(new OrderItem
                {
                    Price = product.Price,
                    Quantity = item.Quantity,
                    Product = new ProductItemOrdered
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        PictureUrl = product.PictureUrl
                    }
                });
            }


            var orderAddress = mapper.Map<OrderAddress>(orderDto.ShipToAddress);
            var deliveryRepo = unitOfWork.GetRepository<DeliveryMethod,int>();
            var deliveryMethod = await deliveryRepo.GetByIdAsync(orderDto.DeliveryMethodId,ct);
            if (deliveryMethod == null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Delivery Method Not Found", $"Delivery Method With Id{orderDto.DeliveryMethodId} Not Found "));
            var SubTotal = orderItems.Sum(x => x.Quantity * x.Price);
            var order = new Order(email, orderItems, orderAddress, deliveryMethod, SubTotal);
            orderRepo.Add(order);
            var result = await unitOfWork.SaveChangesAsync(ct);
            if (result <= 0)
                return Result<OrderToReturnDto>.Fail(Error.Failure("Order Save Failed", "Cannot Create Order"));
            await basketRepository.DeleteBasketAsync(orderDto.BasketId, ct);
            return mapper.Map<OrderToReturnDto>(order);
        }

        
        public async Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetAllDeliveryMehodsAsync(CancellationToken ct = default)
        {
            var deliveryMethods = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync(ct);
            return Result<IReadOnlyList<DeliveryMethodDto>>.Ok(mapper.Map<IReadOnlyList<DeliveryMethodDto>>(deliveryMethods));
        }

        public async Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersAsync(string email, CancellationToken ct)
        {
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(new OrderSpecifications(email), ct);
            return Result<IReadOnlyList<OrderToReturnDto>>.Ok(mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));

        }

        public async Task<Result<OrderToReturnDto>> GetOrderByIdAndEmailAsync(Guid Id, string email, CancellationToken ct)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(new OrderSpecifications(Id, email));
            if (order == null)
            {
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Order Not Found", $"Order With Id {Id} Not Found "));
            }
            return mapper.Map<OrderToReturnDto>(order);
        }
    }
}

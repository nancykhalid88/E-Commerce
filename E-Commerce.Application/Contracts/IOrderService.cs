using E_Commerce.Application.Common;
using E_Commerce.Application.DTOS.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IOrderService
    {
        Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct);
        Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetAllDeliveryMehodsAsync(CancellationToken ct = default);
        Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersAsync(string email, CancellationToken ct);
        Task<Result<OrderToReturnDto>> GetOrderByIdAndEmailAsync(Guid Id, string email, CancellationToken ct);
    }
}

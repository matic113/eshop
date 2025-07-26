using ErrorOr;
using eshop.Application.Dtos;
using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Services
{
    public interface IOrderService
    {
        Task<ErrorOr<OrderCheckoutDto>> CheckoutAsync(Guid userId, Guid shippingAddressId, string paymentMethod);
        Task<OrderHistoryLookupDto> GetOrdersHistoryAsync(Guid orderId);
    }
}

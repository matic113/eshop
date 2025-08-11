using ErrorOr;
using eshop.Application.Dtos;
using eshop.Domain.Entities;
using eShop.Application.Dtos.Orders;

namespace eshop.Application.Contracts.Services
{
    public interface IOrderService
    {
        Task<ErrorOr<OrderCheckoutDto>> CheckoutAsync(Guid userId,
            Guid shippingAddressId, string paymentMethod, string? couponCode);
        Task<OrderHistoryLookupDto> GetOrdersHistoryAsync(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int limit);
        Task<PagedList<GetOrderDto>> GetAllOrdersAsync(
            string period, int page, int pageSize);
    }
}

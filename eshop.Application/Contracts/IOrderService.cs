using ErrorOr;
using eshop.Application.Dtos;

namespace eshop.Application.Contracts
{
    public interface IOrderService
    {
        Task<ErrorOr<OrderCheckoutDto>> CheckoutAsync(Guid userId, Guid shippingAddressId, string paymentMethod);
    }
}

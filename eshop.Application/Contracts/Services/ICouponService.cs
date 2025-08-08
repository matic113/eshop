using ErrorOr;
using eshop.Application.Dtos;
using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Services
{
    public interface ICouponService
    {
        Task<ErrorOr<CouponDto>> CreateCouponAsync(
            string couponCode,
            string couponType,
            DateTime expirationDate,
            int usageTimes,
            int timesPerUser,
            decimal discountValue,
            decimal maxDiscount);

        Task<ErrorOr<CartPriceDto>> ValidateAndCalculateDiscountAsync(Guid userId,
            string couponCode,
            IEnumerable<CartItem> cartItems);
    }
}

using ErrorOr;
using eshop.Application.Dtos;

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
    }
}

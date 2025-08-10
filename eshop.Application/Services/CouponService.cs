using ErrorOr;
using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using eshop.Application.Errors;
using eshop.Application.Extensions;
using eshop.Domain.Entities;
using eshop.Domain.Enums;

namespace eshop.Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(ICouponRepository couponRepository, IUnitOfWork unitOfWork)
        {
            _couponRepository = couponRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<CouponDto>> CreateCouponAsync(
            string couponCode,
            string couponType,
            DateTime expirationDate,
            int usageTimes,
            int timesPerUser,
            decimal discountValue,
            decimal maxDiscount)
        {
            var isValidCouponType = Enum.TryParse<CouponType>(couponType, out var couponTypeResult);

            if (!isValidCouponType)
            {
                return CouponErrors.InvalidCouponType;
            }

            var existingCoupon = await _couponRepository.CheckExistsAsync(c => c.CouponCode == couponCode);

            if (existingCoupon)
            {
                return CouponErrors.CouponAlreadyExists;
            }

            var coupon = new Coupon
            {
                Id = Guid.NewGuid(),
                CouponCode = couponCode,
                CouponType = couponTypeResult,
                ExpiresAt = expirationDate,
                UsagesLeft = usageTimes,
                TimesUsed = 0,
                TimesPerUser = timesPerUser,
                DiscountValue = discountValue,
                MaxDiscount = maxDiscount
            };

            await _couponRepository.AddAsync(coupon);
            await _unitOfWork.SaveChangesAsync();

            return new CouponDto
            {
                Id = coupon.Id,
                CouponCode = coupon.CouponCode,
                CouponType = coupon.CouponType.ToString(),
                ExpiresAt = coupon.ExpiresAt,
                UsagesLeft = coupon.UsagesLeft,
                TimesUsed = coupon.TimesUsed,
                TimesPerUser = coupon.TimesPerUser,
                DiscountValue = coupon.DiscountValue,
                MaxDiscount = coupon.MaxDiscount
            };
        }

        public async Task<ErrorOr<CartPriceDto>> ValidateAndCalculateDiscountAsync(Guid userId,
            string couponCode, IEnumerable<CartItem> cartItems)
        {
            var coupon = await _couponRepository.GetCouponByCodeWithUserUsageAsync(couponCode, userId);

            if (coupon is null)
            {
                return CouponErrors.NotFound;
            }

            if (coupon.ExpiresAt < DateTime.UtcNow)
            {
                return CouponErrors.CouponExpired;
            }

            if (coupon.UsagesLeft <= 0)
            {
                return CouponErrors.CouponUsageLimitExceeded;
            }

            if (coupon.CouponsUsages.Any(cu => cu.UserId == userId && cu.TimesUsed >= coupon.TimesPerUser))
            {
                return CouponErrors.UserLimitExcedeed;
            }

            var totalItemsPrice = cartItems.Sum(item =>
            {
                var baseUnitPrice = item.Product.Price;
                var finalUnitPrice = baseUnitPrice * (1 - item.Product.DiscountPercentage / 100m);
                return finalUnitPrice * item.Quantity;
            }).RoundMoney();

            var couponDiscount = coupon.CouponType switch
            {
                CouponType.Percentage => (totalItemsPrice * (coupon.DiscountValue / 100m)).RoundMoney(),
                CouponType.FixedAmount => coupon.DiscountValue,
                _ => 0
            };

            if (couponDiscount > coupon.MaxDiscount)
            {
                couponDiscount = coupon.MaxDiscount;
            }

            if (couponDiscount > totalItemsPrice)
            {
                couponDiscount = totalItemsPrice;
            }

            var finalPrice = totalItemsPrice - couponDiscount;

            var cartPriceDto = new CartPriceDto
            {
                OriginalPrice = totalItemsPrice,
                DiscountAmount = couponDiscount,
                FinalPrice = finalPrice,
                CouponId = coupon.Id,
                CouponCode = coupon.CouponCode,
                CouponType = coupon.CouponType.ToString(),
                CouponValue = coupon.DiscountValue
            };

            return cartPriceDto;
        }
    }
}

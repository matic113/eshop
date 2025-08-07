using ErrorOr;
using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using eshop.Application.Errors;
using eshop.Domain.Entities;
using eshop.Domain.Enums;

namespace eshop.Application.Services
{
    public class CouponService : ICouponService
    {
        private readonly IGenericRepository<Coupon> _couponRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CouponService(IGenericRepository<Coupon> couponRepository, IUnitOfWork unitOfWork)
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
                TimesPerUser = coupon.TimesPerUser,
                DiscountValue = coupon.DiscountValue,
                MaxDiscount = coupon.MaxDiscount
            };
        }
    }
}

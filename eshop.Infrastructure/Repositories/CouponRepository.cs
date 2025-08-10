using eshop.Application.Contracts.Repositories;
using eshop.Application.Dtos;
using eshop.Domain.Entities;
using eshop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Repositories
{
    public class CouponRepository : GenericRepository<Coupon>, ICouponRepository
    {
        public CouponRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<CouponDto>> GetAllCouponsAsync()
        {
            var coupons = await _context.Coupons
                .Select(c => new CouponDto
                {
                    Id = c.Id,
                    CouponCode = c.CouponCode,
                    CouponType = c.CouponType.ToString(),
                    ExpiresAt = c.ExpiresAt,
                    UsagesLeft = c.UsagesLeft,
                    TimesUsed = c.TimesUsed,
                    TimesPerUser = c.TimesPerUser,
                    DiscountValue = c.DiscountValue,
                    MaxDiscount = c.MaxDiscount
                })
                .ToListAsync();

            return coupons;
        }

        public async Task<Coupon?> GetCouponByCodeWithUserUsageAsync(string couponCode, Guid userId)
        {
            var coupon = await _context.Coupons
                .Include(c => c.CouponsUsages.Where(cu => cu.UserId == userId))
                .Where(c => c.CouponCode == couponCode)
                .FirstOrDefaultAsync();

            return coupon;
        }

        public async Task RecordCouponUsageAsync(string couponCode, Guid userId)
        {
            var coupon = await _context.Coupons
                .Include(c => c.CouponsUsages.Where(cu => cu.UserId == userId))
                .FirstOrDefaultAsync(c => c.CouponCode == couponCode);

            if (coupon == null)
            {
                throw new ArgumentException("Coupon not found.");
            }

            coupon.UsagesLeft -= 1;
            coupon.TimesUsed += 1;

            if (!coupon.CouponsUsages.Any(cu => cu.UserId == userId))
            {
                coupon.CouponsUsages.Add(new CouponsUsage
                {
                    UserId = userId,
                    CouponId = coupon.Id,
                    TimesUsed = 1,
                    LastUsedAt = DateTime.UtcNow
                });
            }
            else
            {
                var usage = coupon.CouponsUsages.First(cu => cu.UserId == userId);
                usage.TimesUsed += 1;
                usage.LastUsedAt = DateTime.UtcNow;
            }

            return;
        }
    }
}

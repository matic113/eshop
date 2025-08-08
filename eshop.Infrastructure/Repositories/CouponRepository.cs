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
    }
}

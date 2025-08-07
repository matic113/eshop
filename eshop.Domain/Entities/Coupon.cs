using eshop.Domain.Enums;

namespace eshop.Domain.Entities
{
    public class Coupon : IBaseEntity
    {
        public Guid Id { get; set; }
        public required string CouponCode { get; set; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public int UsagesLeft { get; set; }
        public int TimesPerUser { get; set; }
        public CouponType CouponType { get; set; }
        public decimal DiscountValue { get; set; } = 0;
        public decimal MaxDiscount { get; set; }

        // Navigation properties
        public ICollection<CouponsUsage> CouponsUsages { get; set; } = new List<CouponsUsage>();
    }
}

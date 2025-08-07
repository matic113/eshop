namespace eshop.Domain.Entities
{
    public class CouponsUsage : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CouponId { get; set; }
        public int TimesUsed { get; set; }
        public DateTime LastUsedAt { get; set; }

        // Navigation properties
        public Coupon Coupon { get; set; } = null!;
    }
}

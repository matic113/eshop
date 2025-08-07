namespace eshop.Application.Dtos
{
    public class CouponDto
    {
        public Guid Id { get; set; }
        public string CouponCode { get; set; } = null!;
        public string CouponType { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public int UsagesLeft { get; set; }
        public int TimesPerUser { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal MaxDiscount { get; set; }
    }
}

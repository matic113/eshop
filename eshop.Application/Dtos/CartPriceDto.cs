using System.Runtime;

namespace eshop.Application.Dtos
{
    public class CartPriceDto
    {
        public Guid CartId { get; set; }
        public Guid CouponId { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public string? CouponCode { get; set; }
        public string? CouponType { get; set; }
        public decimal? CouponValue { get; set; }
    }
}

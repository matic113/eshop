namespace eShop.Application.Dtos.Orders;
public sealed class GetOrderDto
{
    public required Guid OrderId { get; set; }
    public required string OrderCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required string Status { get; set; }
    public required decimal TotalPrice { get; set; }
    public required decimal ShippingPrice { get; set; }
    public required decimal DiscountAmout { get; set; }
    public required string CouponCode { get; set; }
    public required string PaymentMethod { get; set; }
    public required CustomerInfoDto CustomerInfo { get; set; }
    public required ShippingDetails ShippingDetails { get; set; }
}

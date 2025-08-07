namespace eshop.Domain.Entities
{
    public class OrderItem : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmount { get; set; }

        // Navigational properties
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        // Product
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}

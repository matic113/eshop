namespace eshop.Domain.Entities
{
    public class OrderItem : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid SellerId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Navigational properties
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;
        // Product
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}

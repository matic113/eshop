
namespace eshop.Domain.Entities
{
    public class CartItem : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

        // Navigational properties
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}

namespace eshop.Domain.Entities
{
    public class Cart : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        // Navigational properties
        public ICollection<CartItem> CartItems { get; set; } = new HashSet<CartItem>();
    }
}

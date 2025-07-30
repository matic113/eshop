
namespace eshop.Domain.Entities
{
    public class Category : IBaseEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        // Navigational properties
        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}

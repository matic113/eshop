namespace eshop.Domain.Entities
{
    public class Review : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public short Rating { get; set; } = 0;

        // Navigational properties
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}

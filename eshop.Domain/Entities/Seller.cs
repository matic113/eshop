namespace eshop.Domain.Entities
{
    public class Seller : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string StoreName { get; set; }
    }
}

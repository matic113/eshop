namespace eshop.Domain.Entities
{
    public class Offer : IBaseEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required string CoverUrl { get; set; }
    }
}

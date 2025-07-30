namespace eshop.Domain.Entities
{
    public class Address : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public required string State { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string Apartment { get; set; }
        public required string PhoneNumber { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}

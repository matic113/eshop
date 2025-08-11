namespace eShop.Application.Dtos.Orders
{
    public sealed class ShippingDetails
    {
        public Guid AddressId { get; set; }
        public required string State { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string Apartment { get; set; }
        public required string Notes { get; set; }
        public required string PhoneNumber { get; set; }
    }
}

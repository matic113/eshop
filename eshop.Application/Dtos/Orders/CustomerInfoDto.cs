namespace eShop.Application.Dtos.Orders
{
    public sealed class CustomerInfoDto
    {
        public required Guid CustomerId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
    }
}

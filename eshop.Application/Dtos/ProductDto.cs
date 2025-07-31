namespace eshop.Application.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public required string ProductCode { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string ArabicName { get; set; }
        public required string ArabicDescription { get; set; }
        public required string CoverPictureUrl { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal Weight { get; set; }
        public required string Color { get; set; }
        public float Rating { get; set; }
        public int ReviewsCount { get; set; }
        public short DiscountPercentage { get; set; }
        public Guid SellerId { get; set; }
        public List<string> Categories { get; set; } = [];
    }
}

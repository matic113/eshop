using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using FastEndpoints;

namespace eshop.API.Features.Products
{
    sealed class GetProductEndpoint : EndpointWithoutRequest
    {
        private readonly IGenericRepository<Product> _productRepository;

        public GetProductEndpoint(IGenericRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public override void Configure()
        {
            Get("/api/products/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithTags("Products"));
        }
        public override async Task HandleAsync(CancellationToken c)
        {
            var productId = Route<Guid>("Id");
            var product = await _productRepository.GetByIdAsync(productId);

            if (product is null)
            {
                await SendNotFoundAsync(c);
                return;
            }

            var response = new GetProductResponse
            {
                Id = product.Id,
                ProductCode = product.ProductCode,
                Name = product.Name,
                Description = product.Description,
                CoverPictureUrl = product.CoverPictureUrl,
                Price = product.Price,
                Stock = product.Stock,
                Weight = product.Weight,
                Color = product.Color,
                DiscountPercentage = product.DiscountPercentage,
                SellerId = product.SellerId
            };

            await SendOkAsync(response, c);
        }
        sealed class GetProductResponse
        {
            public Guid Id { get; set; }
            public string ProductCode { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string CoverPictureUrl { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public decimal Weight { get; set; }
            public string Color { get; set; }
            public short DiscountPercentage { get; set; }
            public Guid SellerId { get; set; }
        }
    }
}

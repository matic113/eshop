using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using FastEndpoints;

namespace eshop.API.Features.Products
{
    public class ListAll
    {
        sealed class ListAllProductsEndpoint : EndpointWithoutRequest<IEnumerable<GetProductResponse>>
        {
            private readonly IGenericRepository<Product> _productRepository;

            public ListAllProductsEndpoint(IGenericRepository<Product> productRepository)
            {
                _productRepository = productRepository;
            }

            public override void Configure()
            {
                Get("/api/products");
                AllowAnonymous();
                Description(x => x
                .WithTags("Products"));
            }

            public override async Task HandleAsync(CancellationToken c)
            {
                var products = await _productRepository.GetAllAsync();

                var response = products.Select(p => new GetProductResponse
                {
                    Id = p.Id,
                    ProductCode = p.ProductCode,
                    Name = p.Name,
                    NameArabic = p.NameArabic,
                    Description = p.Description,
                    DescriptionArabic = p.DescriptionArabic,
                    CoverPictureUrl = p.CoverPictureUrl,
                    Price = p.Price,
                    Stock = p.Stock,
                    Weight = p.Weight,
                    Color = p.Color,
                    DiscountPercentage = p.DiscountPercentage,
                    SellerId = p.SellerId
                });

                await SendOkAsync(response, c);
            }
        }

        sealed class GetProductResponse
        {
            public Guid Id { get; set; }
            public string ProductCode { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string NameArabic { get; set; }
            public string DescriptionArabic { get; set; }
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

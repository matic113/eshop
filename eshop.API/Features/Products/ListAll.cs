using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using FastEndpoints;

namespace eshop.API.Features.Products
{
    public class ListAllProductsRequest
    {
        public string? SearchTerm { get; set; }
        public string? Category { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public bool? IsInStock { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ListAll : Endpoint<ListAllProductsRequest, PagedList<ProductDto>>
    {
        private readonly IProductService _productsService;

        public ListAll(IProductService productsService)
        {
            _productsService = productsService;
        }

        public override void Configure()
        {
            Get("/api/products");
            AllowAnonymous();
            Description(x => x
                .WithTags("Products"));
        }

        public override async Task HandleAsync(ListAllProductsRequest r, CancellationToken c)
        {
            var searchRequest = new SearchAndFilterProductsRequest
            {
                SearchTerm = r.SearchTerm,
                CategoryName = r.Category,
                MinPrice = r.MinPrice,
                MaxPrice = r.MaxPrice,
                IsInStock = r.IsInStock,
                SortBy = r.SortBy,
                SortOrder = r.SortOrder,
                Page = r.Page,
                PageSize = r.PageSize
            };

            var products = await _productsService.SearchAndFilterProducts(searchRequest);

            await SendOkAsync(products, c);
        }
    }
}
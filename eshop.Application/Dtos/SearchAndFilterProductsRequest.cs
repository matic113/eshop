namespace eshop.Application.Dtos
{
    public sealed class SearchAndFilterProductsRequest
    {
        // Search
        public string? SearchTerm { get; set; }

        // Filtering
        public Guid? CategoryId { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public bool? IsInStock { get; set; }

        // Sorting
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
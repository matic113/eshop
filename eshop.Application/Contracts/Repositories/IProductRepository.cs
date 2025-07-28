using eshop.Application.Dtos;
using eshop.Domain.Entities;


namespace eshop.Application.Contracts.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<PagedList<ProductDto>> SearchAndFilterProductsAsync(SearchAndFilterProductsRequest request);
        Task UpdateProductsBulkAsync(IEnumerable<Product> products);
    }
}

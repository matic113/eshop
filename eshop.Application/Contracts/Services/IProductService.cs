using eshop.Application.Dtos;

namespace eshop.Application.Contracts.Services
{
    public interface IProductService
    {
        Task<PagedList<ProductDto>> SearchAndFilterProducts(SearchAndFilterProductsRequest request);
    }
}

using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;

namespace eshop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedList<ProductDto>> SearchAndFilterProducts(SearchAndFilterProductsRequest request)
        {
            var products = await _productRepository.SearchAndFilterProductsAsync(request);
            return products;
        }

        public async Task<ProductDto?> UpdateProductAsync(Guid productId, UpdateProductRequest request)
        {
            return await _productRepository.UpdateProductAsync(productId, request);
        }
    }
}

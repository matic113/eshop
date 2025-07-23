using eshop.Domain.Entities;

namespace eshop.Application.Contracts
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task UpdateProductsBulkAsync(IEnumerable<Product> products);
    }
}

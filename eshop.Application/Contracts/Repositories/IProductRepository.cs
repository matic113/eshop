using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task UpdateProductsBulkAsync(IEnumerable<Product> products);
    }
}

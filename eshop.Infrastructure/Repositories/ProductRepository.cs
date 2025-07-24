using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using eshop.Infrastructure.Persistence;

namespace eshop.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task UpdateProductsBulkAsync(IEnumerable<Product> products)
        {
            _context.Products.UpdateRange(products);
            return;
        }
    }
}

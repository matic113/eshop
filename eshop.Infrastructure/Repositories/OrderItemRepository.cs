using eshop.Application.Contracts;
using eshop.Domain.Entities;
using eshop.Infrastructure.Persistence;

namespace eshop.Infrastructure.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task AddOrderItemsBulkAsync(IEnumerable<OrderItem> orderItems)
        {
            await _context.AddRangeAsync(orderItems);

            await _context.SaveChangesAsync();
            return;
        }
    }
}

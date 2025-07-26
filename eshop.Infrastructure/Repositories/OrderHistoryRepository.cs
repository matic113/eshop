using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using eshop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Repositories
{
    public class OrderHistoryRepository : GenericRepository<OrderStatusHistory>, IOrderHistoryRepository
    {
        public OrderHistoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OrderStatusHistory>> GetOrderHistoryByOrderIdAsync(Guid orderId)
        {
            return await _context.OrderStatusHistories
                .Where(h => h.OrderId == orderId)
                .ToListAsync();
        }
    }
}

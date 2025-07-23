using eshop.Application.Contracts;
using eshop.Domain.Entities;
using eshop.Infrastructure.Persistence;

namespace eshop.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}

using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Repositories
{
    public interface IOrderHistoryRepository : IGenericRepository<OrderStatusHistory>
    {
        Task<IEnumerable<OrderStatusHistory>> GetOrderHistoryByOrderIdAsync(Guid orderId);
    }
}

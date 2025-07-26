using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithProductsByIdAsync(Guid orderId);
        Task<Order?> GetOrderWithHistoryByIdAsync(Guid orderId);
    }
}

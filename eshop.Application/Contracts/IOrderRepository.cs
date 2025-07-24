using eshop.Domain.Entities;

namespace eshop.Application.Contracts
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithProductsByIdAsync(Guid orderId);
    }
}

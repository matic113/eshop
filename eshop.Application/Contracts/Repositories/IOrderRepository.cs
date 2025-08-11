using eshop.Domain.Entities;
using eShop.Application.Dtos.Orders;

namespace eshop.Application.Contracts.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithProductsByIdAsync(Guid orderId);
        Task<Order?> GetOrderWithHistoryByIdAsync(Guid orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int limit);
        Task<PagedList<GetOrderDto>> GetAllOrdersAsync(string period, int page, int pageSize);
    }
}

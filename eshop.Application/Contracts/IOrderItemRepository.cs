using eshop.Domain.Entities;

namespace eshop.Application.Contracts
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        Task AddOrderItemsBulkAsync(IEnumerable<OrderItem> OrderItems);
    }
}

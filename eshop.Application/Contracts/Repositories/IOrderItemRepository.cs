using eshop.Domain.Entities;

namespace eshop.Application.Contracts.Repositories
{
    public interface IOrderItemRepository : IGenericRepository<OrderItem>
    {
        Task AddOrderItemsBulkAsync(IEnumerable<OrderItem> OrderItems);
    }
}

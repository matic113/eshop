using eshop.Domain.Entities;

namespace eshop.Application.Contracts
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetCartByUserIdAsync(Guid userId);
        Task<CartItem?> AddItemToCartAsync(Guid userId, Guid productId, int quantity);
        Task<bool> RemoveItemFromCartAsync(Guid userId, Guid itemId);
        Task<CartItem?> UpdateItemQuantityAsync(Guid userId, Guid itemId, int quantity);
        Task<Cart> CreateEmptyCartAsync(Guid userId);
    }
}

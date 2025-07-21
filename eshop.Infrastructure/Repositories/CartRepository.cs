using eshop.Application.Contracts;
using eshop.Domain.Entities;
using eshop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return cart;
        }
        public async Task<CartItem?> AddItemToCartAsync(Guid userId, Guid productId, int quantity)
        {
            bool productExists = await _context.Products.AnyAsync(p => p.Id == productId);

            if (!productExists)
            {
                return null;
            }

            var cart = await GetCartByUserIdAsync(userId) ?? await CreateEmptyCartAsync(userId);

            var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem is not null)
            {
                var newQuantity = existingItem.Quantity + quantity;
                existingItem.Quantity = newQuantity;
                await UpdateAsync(cart);
                return existingItem;
            }

            var newItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity
            };

            // Save cartItem to the database
            await _context.CartItems.AddAsync(newItem);
            cart.CartItems.Add(newItem);

            await _context.SaveChangesAsync();

            return newItem;
        }
        public async Task<bool> RemoveItemFromCartAsync(Guid userId, Guid itemId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return false;
            }

            var item = cart.CartItems.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
            {
                return false;
            }

            cart.CartItems.Remove(item);
            await UpdateAsync(cart);

            return true;
        }

        public async Task<CartItem?> UpdateItemQuantityAsync(Guid userId, Guid itemId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                return null;
            }

            var item = cart.CartItems.FirstOrDefault(i => i.Id == itemId);

            if (item == null)
            {
                return null;
            }

            item.Quantity = quantity;
            await UpdateAsync(cart);

            return item;
        }

        public async Task<Cart> CreateEmptyCartAsync(Guid userId)
        {
            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CartItems = new List<CartItem>()
            };

            await AddAsync(cart);
            return cart;
        }
    }
}

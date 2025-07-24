using System.IO.Pipes;
using ErrorOr;
using eshop.Application.Contracts;
using eshop.Application.Errors;
using eshop.Domain.Entities;

namespace eshop.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IGenericRepository<CartItem> _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, IGenericRepository<CartItem> cartItemRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<ErrorOr<CartItem>> AddItemToUserCartAsync(Guid userId,
            Guid productId,
            int quantity = 1)
        {
            var cart = await _cartRepository.GetCartWithProductsByUserIdAsync(userId);

            if (cart is null)
            {
                return CartErrors.NotFound;
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if (product is null)
            {
                return CartErrors.ProductNotFound;
            }

            if (product.Stock == 0)
            {
                return CartErrors.ProductOutOfStock;
            }

            var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem is not null)
            {
                var requestedQuantity = existingItem.Quantity + quantity;

                if (requestedQuantity > product.Stock)
                {
                    return CartErrors.NotEnoughStock(requestedQuantity, product.Stock);
                }

                existingItem.Quantity += quantity;
                await _unitOfWork.SaveChangesAsync();
                return existingItem;
            }

            if (quantity > product.Stock)
            {
                return CartErrors.NotEnoughStock(quantity, product.Stock);
            }

            var newItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity
            };

            await _cartItemRepository.AddAsync(newItem);

            cart.CartItems.Add(newItem);

            await _unitOfWork.SaveChangesAsync();

            return newItem;
        }
    }
}

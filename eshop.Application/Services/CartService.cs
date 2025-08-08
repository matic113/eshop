using ErrorOr;
using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;
using eshop.Application.Dtos;
using eshop.Application.Errors;
using eshop.Application.Extensions;
using eshop.Domain.Entities;
using eshop.Domain.Enums;

namespace eshop.Application.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IGenericRepository<CartItem> _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, IGenericRepository<CartItem> cartItemRepository, ICouponRepository couponRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _cartItemRepository = cartItemRepository;
            _couponRepository = couponRepository;
        }

        public async Task<ErrorOr<CartItem>> AddItemToUserCartAsync(Guid userId,
            Guid productId,
            int quantity = 1)
        {
            var cart = await _cartRepository.GetCartWithProductsByUserIdAsync(userId);

            if (cart is null)
            {
                cart = await _cartRepository.CreateEmptyCartAsync(userId);
                await _unitOfWork.SaveChangesAsync();
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

        public async Task<ErrorOr<CartPriceDto>> ApplyCouponToCartAsync(Guid userId, string couponCode)
        {
            var cart = await _cartRepository.GetCartWithProductsByUserIdAsync(userId);

            if (cart is null)
            {
                await _cartRepository.CreateEmptyCartAsync(userId);
                await _unitOfWork.SaveChangesAsync();

                return CartErrors.Empty;
            }

            if (cart.CartItems.Count == 0)
            {
                return CartErrors.Empty;
            }   

            var coupon = await _couponRepository.GetCouponByCodeWithUserUsageAsync(couponCode, userId);

            if (coupon is null)
            {
                return CouponErrors.NotFound;
            }

            if (coupon.ExpiresAt < DateTime.UtcNow)
            {
                return CouponErrors.CouponExpired;
            }

            if (coupon.UsagesLeft <= 0)
            {
                return CouponErrors.CouponUsageLimitExceeded;
            }

            if (coupon.CouponsUsages.Any(cu => cu.UserId == userId && cu.TimesUsed >= coupon.TimesPerUser))
            {
                return CouponErrors.UserLimitExcedeed;
            }

            var totalItemsPrice = cart.CartItems.Sum(item =>
            {
                var baseUnitPrice = item.Product.Price;
                var finalUnitPrice = baseUnitPrice * (1 - item.Product.DiscountPercentage / 100m);
                return finalUnitPrice * item.Quantity;
            }).RoundMoney();

            var couponDiscount = coupon.CouponType switch
            {
                CouponType.Percentage => (totalItemsPrice * (coupon.DiscountValue / 100m)).RoundMoney(),
                CouponType.FixedAmount => coupon.DiscountValue,
                _ => 0
            };

            if (couponDiscount > coupon.MaxDiscount)
            {
                couponDiscount = coupon.MaxDiscount;
            }

            if (couponDiscount > totalItemsPrice)
            {
                couponDiscount = totalItemsPrice;
            }

            var finalPrice = totalItemsPrice - couponDiscount;
            var cartPriceDto = new CartPriceDto
            {
                CartId = cart.Id,
                OriginalPrice = totalItemsPrice,
                DiscountAmount = couponDiscount,
                FinalPrice = finalPrice,
                CouponCode = coupon.CouponCode,
                CouponType = coupon.CouponType.ToString(),
                CouponValue = coupon.DiscountValue
            };

            return cartPriceDto;
        }

        public async Task<ErrorOr<ItemDecrementDto>> DecrementItemQuantityInUserCartAsync(Guid userId, Guid itemId, int quantity = 1)
        {
            var cart = await _cartRepository.GetCartWithProductsByUserIdAsync(userId);

            if (cart is null)
            {
                return CartErrors.NotFound;
            }

            var item = cart.CartItems.FirstOrDefault(i => i.Id == itemId);
            if (item is null)
            {
                return CartErrors.ItemNotFound;
            }

            item.Quantity -= quantity;

            if (item.Quantity < 1)
            {
                cart.CartItems.Remove(item);
                await _unitOfWork.SaveChangesAsync();
                return new ItemDecrementDto
                {
                    WasRemoved = true,
                    CartItem = item
                };
            }

            await _unitOfWork.SaveChangesAsync();

            return new ItemDecrementDto
            {
                WasRemoved = false,
                CartItem = item
            };
        }

    }
}

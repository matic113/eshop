using eshop.Application.Contracts;
using eshop.Application.Dtos;
using eshop.Domain.Entities;
using eshop.Domain.Enums;
using System.Collections.Concurrent;
using ErrorOr;
using eshop.Application.Errors;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Contracts.Services;

namespace eshop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPublicCodeGenerator _publicCodeGenerator;
        private readonly IPaymobService _paymobService;

        private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _productLocks = new ConcurrentDictionary<Guid, SemaphoreSlim>();

        public OrderService(IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IPublicCodeGenerator publicCodeGenerator,
            IAddressRepository addressRepository,
            IProductRepository productRepository,
            IPaymobService paymobService,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _publicCodeGenerator = publicCodeGenerator;
            _addressRepository = addressRepository;
            _productRepository = productRepository;
            _paymobService = paymobService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<OrderCheckoutDto>> CheckoutAsync(Guid userId, Guid shippingAddressId, string paymentMethod)
        {
            var cart = await _cartRepository.GetCartWithProductsByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return OrderErrors.CartIsEmpty;
            }

            var address = await _addressRepository.GetByIdAsync(shippingAddressId);

            if (address is null)
            {
                return OrderErrors.AddressNotFound;
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = _publicCodeGenerator.Generate("ORD"),
                UserId = userId,
                ShippingAddressId = shippingAddressId,
                PaymentMethod = Enum.Parse<PaymentMethod>(paymentMethod)
            };

            await _orderRepository.AddAsync(order);

            var stockUpdateResult = await ProcessOrderItemsAndStock(order, cart.CartItems);

            if (stockUpdateResult.IsError)
            {
                return stockUpdateResult.Errors;
            }

            // Finalize order details
            order.TotalPrice = order.OrderItems.Sum(oi => oi.Price * oi.Quantity);

            order.Status = order.PaymentMethod == PaymentMethod.CashOnDelivery
                ? OrderStatus.Processing
                : OrderStatus.Pending;

            order.OrderStatusHistories.Add(new OrderStatusHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                OrderCode = order.OrderNumber,
                OrderStatus = OrderStatus.Pending,
                ChangeDate = DateTime.UtcNow
            });


            // Handle Payment
            if (order.PaymentMethod == PaymentMethod.CashOnDelivery)
            {

                order.OrderStatusHistories.Add(new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    OrderCode = order.OrderNumber,
                    OrderStatus = OrderStatus.Processing,
                    ChangeDate = DateTime.UtcNow
                });

                await _cartRepository.ClearUserCartAsync(userId);
                await _unitOfWork.SaveChangesAsync();

                return new OrderCheckoutDto { Order = order };
            }
            else
            {
                await _unitOfWork.SaveChangesAsync();

                var paymentIntent = await _paymobService.CreatePaymentIntentAsync(order);
                if (paymentIntent is null)
                {
                    return OrderErrors.PaymentIntentFailed;
                }

                return new OrderCheckoutDto
                {
                    Order = order,
                    UnifiedCheckoutUrl = paymentIntent.UnifiedChechoutUrl,
                    PaymentClientSecret = paymentIntent.ClientSecret
                };
            }
        }

        private async Task<ErrorOr<Success>> ProcessOrderItemsAndStock(Order order, IEnumerable<CartItem> cartItems)
        {
            var productsToUpdate = new List<Product>();
            var locks = new List<SemaphoreSlim>();

            try
            {
                // Acquire locks first to prevent deadlocks
                foreach (var productId in cartItems.Select(ci => ci.ProductId).Distinct().OrderBy(id => id))
                {
                    var productLock = _productLocks.GetOrAdd(productId, new SemaphoreSlim(1, 1));
                    await productLock.WaitAsync();
                    locks.Add(productLock);
                }

                // Now process items
                foreach (var cartItem in cartItems)
                {
                    var product = await _productRepository.GetByIdAsync(cartItem.ProductId);

                    if (product == null) return OrderErrors.ProductNotFound(cartItem.ProductId);
                    if (product.Stock < cartItem.Quantity) return OrderErrors.InsufficientStock(product.Name, product.Stock);

                    order.OrderItems.Add(new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = product.Price,
                        SellerId = product.SellerId,
                    });

                    // Only decrement stock for COD here. For Paymob, decrement after successful webhook.
                    if (order.PaymentMethod == PaymentMethod.CashOnDelivery)
                    {
                        product.Stock -= cartItem.Quantity;
                        productsToUpdate.Add(product);
                    }
                }

                if (productsToUpdate.Any())
                {
                    await _productRepository.UpdateProductsBulkAsync(productsToUpdate);
                }
            }
            finally
            {
                foreach (var productLock in locks) { productLock.Release(); }
            }

            return Result.Success;
        }

        public async Task<OrderHistoryLookupDto> GetOrdersHistoryAsync(Guid orderId)
        {
            var order = await _orderRepository.GetOrderWithHistoryByIdAsync(orderId);

            if (order == null)
            {
                return new OrderHistoryLookupDto
                {
                    OrderStatusHistories = []
                };
            }

            return new OrderHistoryLookupDto
            {
                OrderCode = order.OrderNumber,
                OrderStatusHistories = order.OrderStatusHistories
            };
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int limit)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId, limit);
            return orders;
        }
    }
}

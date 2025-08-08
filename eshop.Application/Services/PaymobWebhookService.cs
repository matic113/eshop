using ErrorOr;
using eshop.Application.Contracts;
using eshop.Application.Contracts.Repositories;
using eshop.Application.Dtos.Paymob;
using eshop.Domain.Entities;
using eshop.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace eshop.Application.Services
{
    public class PaymobWebhookService : IPaymobWebhookService
    {
        private readonly IPaymobHmacValidator _hmacValidator;
        private readonly ILogger<PaymobWebhookService> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderHistoryRepository _orderHistoryRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymobWebhookService(IPaymobHmacValidator hmacValidator,
            ILogger<PaymobWebhookService> logger,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork,
            ICartRepository cartRepository,
            IOrderHistoryRepository orderHistoryRepository,
            ICouponRepository couponRepository)
        {
            _hmacValidator = hmacValidator;
            _logger = logger;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
            _orderHistoryRepository = orderHistoryRepository;
            _couponRepository = couponRepository;
        }

        public async Task<ErrorOr<Success>> ProcessTransactionAsync(TransactionProcessedCallbackDto transactionDto, string recievedHmac)
        {
            if (!_hmacValidator.IsValid(transactionDto.Obj, recievedHmac))
            {
                _logger.LogWarning("Invalid HMAC signature received for Paymob webhook.");
                return Error.Unauthorized("Webhook.InvalidHmac", "Invalid HMAC signature.");
            }

            var orderId = transactionDto.Obj.Order.MerchantOrderId;

            if (string.IsNullOrEmpty(orderId))
            {
                _logger.LogWarning("Received Paymob webhook with empty Order ID.");
                return Error.Validation("Webhook.EmptyOrderId", "Order ID cannot be empty.");
            }

            var orderIdGuid = Guid.Parse(orderId);
            var order = await _orderRepository.GetOrderWithProductsByIdAsync(orderIdGuid);

            if (order == null)
            {
                _logger.LogWarning("Received Paymob webhook for non-existent order ID: {OrderId}", orderId);
                return Error.NotFound("Webhook.OrderNotFound", $"Order with ID '{orderId}' not found.");
            }

            if (order.Status != OrderStatus.Pending)
            {
                _logger.LogInformation("Received duplicate Paymob webhook for already processed order: {orderId}", orderId);
                return Result.Success;
            }

            // 4. Check the transaction status.
            if (transactionDto.Obj.Success)
            {
                _logger.LogInformation("Payment successful for order: {orderId}. Processing...", orderId);

                // 5. Update the order status.
                order.Status = OrderStatus.Processing;

                await _orderHistoryRepository.AddAsync(new OrderStatusHistory
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    OrderCode = order.OrderNumber,
                    OrderStatus = OrderStatus.Processing,
                    ChangeDate = DateTime.UtcNow
                });

                // 6. Decrement product stock.
                foreach (var item in order.OrderItems)
                {
                    // It's crucial to lock here as well to prevent race conditions.
                    var product = item.Product;
                    if (product != null && product.Stock >= item.Quantity)
                    {
                        product.Stock -= item.Quantity;
                    }
                    else
                    {
                        _logger.LogError("Insufficient stock for product {ProductId} in order {orderId} during webhook processing.", item.ProductId, orderId);
                        // Mark order as failed or handle this critical error.
                        order.Status = OrderStatus.Failed;
                        break;
                    }
                }

                // Mark the coupon as used if applicable
                if (!string.IsNullOrEmpty(order.CouponCode))
                {
                    await _couponRepository.RecordCouponUsageAsync(order.CouponCode, order.UserId);
                }

                await _cartRepository.ClearUserCartAsync(order.UserId);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully processed order: {orderId}", orderId);
                // TODO: Send Email?
            }
            else
            {
                _logger.LogWarning("Payment failed for order: {orderId}", orderId);
                order.Status = OrderStatus.Failed;
                await _unitOfWork.SaveChangesAsync();
            }

            return Result.Success;
        }
    }
}

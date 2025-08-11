using eshop.Application.Contracts.Repositories;
using eshop.Domain.Entities;
using eshop.Infrastructure.Extensions;
using eshop.Infrastructure.Persistence;
using eShop.Application.Dtos.Orders;
using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Order?> GetOrderWithProductsByIdAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return order;
        }
        public async Task<Order?> GetOrderWithHistoryByIdAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderStatusHistories)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return order;
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(Guid userId, int limit)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }
        public async Task<PagedList<GetOrderDto>> GetAllOrdersAsync(string period, int page, int pageSize)
        {
            var daysToSubtract = period switch
            {
                "1d" => 1,
                "3d" => 3,
                "7d" => 7,
                "30d" => 30,
                _ => 0
            };
            var startDate = DateTime.UtcNow.AddDays(-daysToSubtract);

            // Use startDate.Date to ensure we only consider the date part

            var query = _context.Orders
                .Where(o => o.CreatedAt >= startDate.Date)
                .Include(o => o.ShippingAddress)
                .OrderByDescending(o => o.CreatedAt);

            var ordersWithUsersInfo = query.Join(
                _context.Users,
                order => order.UserId,
                user => user.Id,
                (order, user) => new GetOrderDto
                {
                    OrderId = order.Id,
                    OrderCode = order.OrderNumber,
                    ShippingPrice = order.ShippingPrice,
                    DiscountAmout = order.DiscountAmount,
                    TotalPrice = order.TotalPrice,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt,
                    Status = order.Status.ToString(),
                    CouponCode = order.CouponCode ?? "",
                    PaymentMethod = order.PaymentMethod.ToString(),
                    CustomerInfo = new CustomerInfoDto
                    {
                        CustomerId = user.Id,
                        FullName = $"{user.FirstName} {user.LastName}",
                        PhoneNumber = user.PhoneNumber ?? "",
                        Email = user.Email ?? "",
                    },
                    ShippingDetails = new ShippingDetails
                    {
                        AddressId = order.ShippingAddress.Id,
                        State = order.ShippingAddress.State,
                        City = order.ShippingAddress.City,
                        Street = order.ShippingAddress.Street,
                        Apartment = order.ShippingAddress.Apartment,
                        Notes = order.ShippingAddress.Notes,
                        PhoneNumber = order.ShippingAddress.PhoneNumber,
                    }
                });

            return await ordersWithUsersInfo.ToPagedListAsync(page, pageSize);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eshop.Domain.Enums;

namespace eshop.Domain.Entities
{
    public class Order : IBaseEntity
    {
        public Guid Id { get; set; }
        public required string OrderNumber { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Default status
        public decimal TotalPrice { get; set; }
        public Guid ShippingAddressId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigational properties
        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new HashSet<OrderStatusHistory>();
        public Address ShippingAddress { get; set; } = null!;
    }
}

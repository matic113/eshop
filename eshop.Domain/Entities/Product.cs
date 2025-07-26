using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Domain.Entities
{
    public class Product : IBaseEntity
    {
        public Guid Id { get; set; }
        public required string ProductCode { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public int Stock { get; set; }
        public required string CoverPictureUrl { get; set; }
        public decimal Weight { get; set; }
        public string Color { get; set; }
        public short DiscountPercentage { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Navigational properties
        public ICollection<Category> Categories{ get; set; } = new HashSet<Category>();
        // Pictures
        public ICollection<ProductPicture> ProductPictures { get; set; } = new HashSet<ProductPicture>();
        // Reviews
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
        // Order Items
        public ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        // Seller
        public Guid SellerId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Domain.Entities
{
    public class ProductPicture : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public required string PictureUrl { get; set; }

        // Navigational property
        public Product Product { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eshop.Application.Dtos
{
    public class UpdateProductRequest
    {
        public string? Name { get; set; }
        public string? NameArabic { get; set; }
        public string? Description { get; set; }
        public string? DescriptionArabic { get; set; }
        public string? CoverPictureUrl { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public decimal? Weight { get; set; }
        public string? Color { get; set; }
        public int? DiscountPercentage { get; set; }
        public List<Guid>? CategoryIds { get; set; }
        public List<string>? ProductPictures { get; set; }
    }
}

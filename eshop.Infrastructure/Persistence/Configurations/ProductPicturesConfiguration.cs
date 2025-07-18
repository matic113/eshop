using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class ProductPicturesConfiguration : IEntityTypeConfiguration<ProductPictures>
    {
        public void Configure(EntityTypeBuilder<ProductPictures> builder)
        {
            builder.HasKey(pp => pp.Id);

            builder.Property(pp => pp.PictureUrl)
                .IsRequired();
        }
    }
}

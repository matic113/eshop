using eshop.Domain.NonKeyed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class ProductFtsResultConfiguration : IEntityTypeConfiguration<ProductFtsResult>
    {
        public void Configure(EntityTypeBuilder<ProductFtsResult> builder)
        {
            builder.HasNoKey();
            builder.ToTable("ProductFtsResults", t => t.ExcludeFromMigrations());
        }
    }
}

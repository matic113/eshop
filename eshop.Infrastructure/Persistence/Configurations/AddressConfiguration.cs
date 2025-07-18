using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasIndex(a => a.UserId);

            builder.Property(a => a.State)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(a => a.City)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(a => a.Street)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Apartment)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.PhoneNumber)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(a => a.Notes)
                .HasColumnType("text");

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

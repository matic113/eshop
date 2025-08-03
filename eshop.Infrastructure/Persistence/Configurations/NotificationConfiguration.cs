using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.HasIndex(n => n.UserId);

            builder.Property(n => n.Text)
                .IsRequired()
                .HasMaxLength(250);

            // UserId can be null, meaning the notification is for all users
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .IsRequired(false) 
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

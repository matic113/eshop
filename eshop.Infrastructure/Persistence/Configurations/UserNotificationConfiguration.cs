using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eshop.Infrastructure.Persistence.Configurations
{
    public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
    {
        public void Configure(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasKey(un => un.Id);

            builder.HasIndex(un => new { un.UserId, un.NotificationId});

            builder.HasOne(un => un.Notification)
            .WithMany() // A Notification can have many UserNotification statuses
            .HasForeignKey(un => un.NotificationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
            .WithMany() // A User can have many UserNotification statuses
            .HasForeignKey(un => un.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

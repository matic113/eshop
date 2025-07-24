using eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Persistence
{
    public class SqlServerDbContext : ApplicationDbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Apply Configurations for models
            builder.ApplyConfigurationsFromAssembly(typeof(SqlServerDbContext).Assembly);

            // Set the type of Notes property in Address entity to nvarchar(max) to support Arabic text
            builder.Entity<Address>()
                .Property(e => e.Notes)
                .HasColumnType("nvarchar(max)");

            base.OnModelCreating(builder);
        }
    }
}

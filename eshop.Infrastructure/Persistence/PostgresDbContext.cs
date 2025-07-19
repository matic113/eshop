using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Persistence
{
    public class PostgresDbContext : ApplicationDbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options) { }
    }
}

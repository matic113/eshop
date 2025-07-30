using Microsoft.EntityFrameworkCore;

namespace eshop.Infrastructure.Persistence
{
    public class SqlServerDbContext : ApplicationDbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options) { }
    }
}

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace eshop.Infrastructure.Persistence
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "/eshop.API")
                .AddJsonFile("appsettings.Development.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();
            var connectionString = configuration.GetConnectionString("Monster");

            optionsBuilder.UseSqlServer(connectionString,
                b => b.MigrationsAssembly(typeof(SqlServerDbContext).Assembly.FullName));

            return new SqlServerDbContext(optionsBuilder.Options);
        }
    }
}

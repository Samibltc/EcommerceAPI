using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EcommerceAPI.DataAccess.Context;

public sealed class ECommerceDbContextFactory : IDesignTimeDbContextFactory<ECommerceDbContext>
{
    public ECommerceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ECommerceDbContext>();

        var connectionString =
            Environment.GetEnvironmentVariable("ECOMMERCE_DB")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=EcommerceApiDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(typeof(ECommerceDbContext).Assembly.FullName);
        });

        return new ECommerceDbContext(optionsBuilder.Options);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.DataAccess.Context;
using EcommerceAPI.DataAccess.Abstract;
using EcommerceAPI.DataAccess.Concrete.Ef;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceAPI.DataAccess.DependencyInjection;

public static class DataAccessServiceRegistration
{
    public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ECommerceDb")
            ?? throw new InvalidOperationException("Connection string 'ECommerceDb' not found.");

        services.AddDbContext<ECommerceDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sql =>
            {
                // Keep migrations in DataAccess assembly
                sql.MigrationsAssembly(typeof(ECommerceDbContext).Assembly.FullName);
            });
        });

        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<ICustomerRepository, EfCustomerRepository>();
        services.AddScoped<ICartRepository, EfCartRepository>();

        return services;
    }
}

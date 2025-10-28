using System;
using EcommerceAPI.DataAccess.Identity;
using EcommerceAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.DataAccess.Context;

public class ECommerceDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ECommerceDbContext).Assembly);

        // AppUser 1:1 Customer
        modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Customer)
            .WithOne()
            .HasForeignKey<AppUser>(u => u.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

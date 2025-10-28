using EcommerceAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceAPI.DataAccess.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        builder.Property(c => c.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Phone)
            .HasMaxLength(32);

        builder.HasIndex(c => c.Email).IsUnique();

        builder.Property(c => c.CreatedAt).IsRequired();

        // Owned single address (columns inline on Customers)
        builder.OwnsOne(c => c.Address, nav =>
        {
            nav.Property(a => a.AddressLine1).HasMaxLength(200).HasColumnName("AddressLine1");
            nav.Property(a => a.AddressLine2).HasMaxLength(200).HasColumnName("AddressLine2");
            nav.Property(a => a.City).HasMaxLength(100).HasColumnName("City");
            nav.Property(a => a.StateOrRegion).HasMaxLength(100).HasColumnName("StateOrRegion");
            nav.Property(a => a.PostalCode).HasMaxLength(20).HasColumnName("PostalCode");
            nav.Property(a => a.Country).HasMaxLength(100).HasColumnName("Country");
        });
    }
}
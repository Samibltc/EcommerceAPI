using EcommerceAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcommerceAPI.DataAccess.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.ToTable("CartItems");

        builder.HasKey(ci => ci.Id);
        builder.Property(ci => ci.Id).ValueGeneratedOnAdd();

        builder.Property(ci => ci.Quantity)
            .IsRequired();

        builder.Property(ci => ci.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(ci => ci.CreatedAt)
            .IsRequired();

        // Ensure no duplicate product rows per cart
        builder.HasIndex(ci => new { ci.CartId, ci.ProductId })
            .IsUnique();

        // Relationships
        builder.HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.Product)
            .WithMany() // no navigation on Product for cart items
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional: check constraints for valid quantity and price
        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_CartItems_Quantity_Positive", "[Quantity] > 0");
            t.HasCheckConstraint("CK_CartItems_UnitPrice_NonNegative", "[UnitPrice] >= 0");
        });
    }
}

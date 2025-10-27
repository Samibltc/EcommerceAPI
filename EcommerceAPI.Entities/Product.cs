using EcommerceAPI.Entities.Base;
using EcommerceAPI.Entities.Enums;

namespace EcommerceAPI.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
}

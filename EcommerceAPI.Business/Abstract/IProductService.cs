using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Core.DTOs.Products;

namespace EcommerceAPI.Business.Abstract;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task UpdateAsync(UpdateProductRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Entities;

namespace EcommerceAPI.DataAccess.Abstract
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Product>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Product entity, CancellationToken ct = default);
        Task UpdateAsync(Product entity, CancellationToken ct = default);
        Task DeleteAsync(Product entity, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
    }
}

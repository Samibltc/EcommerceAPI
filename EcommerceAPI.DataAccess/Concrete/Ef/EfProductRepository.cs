using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.DataAccess.Abstract;
using EcommerceAPI.DataAccess.Context;
using EcommerceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.DataAccess.Concrete.Ef
{
    public class EfProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext _context;

        public EfProductRepository(ECommerceDbContext context) => _context = context;

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

        public Task<List<Product>> GetAllAsync(CancellationToken ct = default) =>
            _context.Products.AsNoTracking().ToListAsync(ct);

        public async Task AddAsync(Product entity, CancellationToken ct = default)
        {
            await _context.Products.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Product entity, CancellationToken ct = default)
        {
            _context.Products.Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Product entity, CancellationToken ct = default)
        {
            _context.Products.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        public Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
        {
            var q = _context.Products.AsQueryable().Where(p => p.Name == name);
            if (excludeId.HasValue) q = q.Where(p => p.Id != excludeId.Value);
            return q.AnyAsync(ct);
        }
    }
}

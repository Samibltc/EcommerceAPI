using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.DataAccess.Abstract;
using EcommerceAPI.DataAccess.Context;
using EcommerceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.DataAccess.Concrete.Ef;

public class EfCustomerRepository : ICustomerRepository
{
    private readonly ECommerceDbContext _context;

    public EfCustomerRepository(ECommerceDbContext context) => _context = context;

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _context.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<List<Customer>> GetAllAsync(CancellationToken ct = default) =>
        _context.Customers.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Customer entity, CancellationToken ct = default)
    {
        await _context.Customers.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Customer entity, CancellationToken ct = default)
    {
        _context.Customers.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Customer entity, CancellationToken ct = default)
    {
        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync(ct);
    }

    public Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken ct = default)
    {
        var q = _context.Customers.AsQueryable().Where(c => c.Email == email);
        if (excludeId.HasValue) q = q.Where(c => c.Id != excludeId.Value);
        return q.AnyAsync(ct);
    }
}

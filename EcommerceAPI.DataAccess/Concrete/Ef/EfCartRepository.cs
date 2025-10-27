using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceAPI.DataAccess.Abstract;
using EcommerceAPI.DataAccess.Context;
using EcommerceAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.DataAccess.Concrete.Ef;

public class EfCartRepository : ICartRepository
{
    private readonly ECommerceDbContext _context;

    public EfCartRepository(ECommerceDbContext context) => _context = context;

    public Task<Cart?> GetActiveCartWithItemsAsync(Guid customerId, CancellationToken ct = default) =>
        _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.IsActive, ct);

    public async Task AddAsync(Cart cart, CancellationToken ct = default)
    {
        await _context.Carts.AddAsync(cart, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Cart cart, CancellationToken ct = default)
    {
        // Do not call Update(cart); rely on change tracker to insert new items
        await _context.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
}

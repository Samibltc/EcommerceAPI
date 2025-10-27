using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceAPI.Entities;

namespace EcommerceAPI.DataAccess.Abstract
{
    public interface ICartRepository
    {
        Task<Cart?> GetActiveCartWithItemsAsync(Guid customerId, CancellationToken ct = default);
        Task AddAsync(Cart cart, CancellationToken ct = default);
        Task UpdateAsync(Cart cart, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

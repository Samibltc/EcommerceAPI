using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Core.DTOs.Carts;

namespace EcommerceAPI.Business.Abstract
{
    public interface ICartService
    {
        Task<CartDto> GetActiveCartAsync(Guid customerId, CancellationToken ct = default);
        Task AddItemAsync(Guid customerId, AddCartItemRequest request, CancellationToken ct = default);
        Task RemoveItemAsync(Guid customerId, Guid productId, CancellationToken ct = default);
        Task ClearAsync(Guid customerId, CancellationToken ct = default);
    }
}

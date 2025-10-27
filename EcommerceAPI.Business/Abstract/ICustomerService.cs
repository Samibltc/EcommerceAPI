using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Core.DTOs.Customers;

namespace EcommerceAPI.Business.Abstract
{
    public interface ICustomerService
    {
        Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<CustomerDto>> GetAllAsync(CancellationToken ct = default);
        Task<Guid> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default);
        Task UpdateAsync(UpdateCustomerRequest request, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
    }
}

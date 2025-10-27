using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Entities;

namespace EcommerceAPI.DataAccess.Abstract
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(Customer entity, CancellationToken ct = default);
        Task UpdateAsync(Customer entity, CancellationToken ct = default);
        Task DeleteAsync(Customer entity, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken ct = default);
    }
}

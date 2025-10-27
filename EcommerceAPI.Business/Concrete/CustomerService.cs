using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceAPI.Business.Abstract;
using EcommerceAPI.Core.DTOs.Customers;
using EcommerceAPI.DataAccess.Abstract;
using EcommerceAPI.Entities;

namespace EcommerceAPI.Business.Concrete;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repo;
    private readonly IMapper _mapper;

    public CustomerService(ICustomerRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<CustomerDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<CustomerDto>(entity);
    }

    public async Task<List<CustomerDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<CustomerDto>>(list);
    }

    public async Task<Guid> CreateAsync(CreateCustomerRequest request, CancellationToken ct = default)
    {
        if (await _repo.EmailExistsAsync(request.Email, null, ct))
            throw new InvalidOperationException("Email already exists.");

        var entity = _mapper.Map<Customer>(request);
        await _repo.AddAsync(entity, ct);
        return entity.Id;
    }

    public async Task UpdateAsync(UpdateCustomerRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct) ?? throw new KeyNotFoundException("Customer not found.");
        if (await _repo.EmailExistsAsync(request.Email, request.Id, ct))
            throw new InvalidOperationException("Email already exists.");

        _mapper.Map(request, entity);
        await _repo.UpdateAsync(entity, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Customer not found.");
        await _repo.DeleteAsync(entity, ct);
    }
}

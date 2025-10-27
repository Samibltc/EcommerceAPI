using AutoMapper;
using EcommerceAPI.Business.Abstract;
using EcommerceAPI.Core.DTOs.Products;
using EcommerceAPI.DataAccess.Abstract;
using EcommerceAPI.Entities;
using EcommerceAPI.Entities.Enums;

namespace EcommerceAPI.Business.Concrete;

public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository repo, IMapper mapper)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<ProductDto>(entity);
    }

    public async Task<List<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<ProductDto>>(list);
    }

    public async Task<Guid> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (await _repo.ExistsByNameAsync(request.Name, null, ct))
            throw new InvalidOperationException("Product name already exists.");

        var entity = _mapper.Map<Product>(request);
        entity.Status = ProductStatus.Active;

        await _repo.AddAsync(entity, ct);
        return entity.Id;
    }

    public async Task UpdateAsync(UpdateProductRequest request, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = await _repo.GetByIdAsync(request.Id, ct) ?? throw new KeyNotFoundException("Product not found.");
        if (await _repo.ExistsByNameAsync(request.Name, request.Id, ct))
            throw new InvalidOperationException("Product name already exists.");

        _mapper.Map(request, entity);
        await _repo.UpdateAsync(entity, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Product not found.");
        await _repo.DeleteAsync(entity, ct);
    }
}

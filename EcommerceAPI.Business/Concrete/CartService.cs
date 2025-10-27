using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceAPI.Business.Abstract;
using EcommerceAPI.Core.DTOs.Carts;
using EcommerceAPI.DataAccess.Abstract;
using EcommerceAPI.Entities;

namespace EcommerceAPI.Business.Concrete;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepo;
    private readonly IProductRepository _productRepo;
    private readonly IMapper _mapper;

    public CartService(ICartRepository cartRepo, IProductRepository productRepo, IMapper mapper)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo;
        _mapper = mapper;
    }

    public async Task<CartDto> GetActiveCartAsync(Guid customerId, CancellationToken ct = default)
    {
        var cart = await EnsureActiveCart(customerId, ct);
        return _mapper.Map<CartDto>(cart);
    }

    public async Task AddItemAsync(Guid customerId, AddCartItemRequest request, CancellationToken ct = default)
    {
        var cart = await EnsureActiveCart(customerId, ct);

        var product = await _productRepo.GetByIdAsync(request.ProductId, ct)
                      ?? throw new KeyNotFoundException("Product not found.");

        var existing = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
        if (existing is null)
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,             // ensure FK is set
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            });
        }
        else
        {
            existing.Quantity += request.Quantity;
        }

        await _cartRepo.UpdateAsync(cart, ct);
    }

    public async Task RemoveItemAsync(Guid customerId, Guid productId, CancellationToken ct = default)
    {
        var cart = await EnsureActiveCart(customerId, ct);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId)
                   ?? throw new KeyNotFoundException("Item not found.");
        cart.Items.Remove(item);
        await _cartRepo.UpdateAsync(cart, ct);
    }

    public async Task ClearAsync(Guid customerId, CancellationToken ct = default)
    {
        var cart = await EnsureActiveCart(customerId, ct);
        cart.Items.Clear();
        await _cartRepo.UpdateAsync(cart, ct);
    }

    private async Task<Cart> EnsureActiveCart(Guid customerId, CancellationToken ct)
    {
        var cart = await _cartRepo.GetActiveCartWithItemsAsync(customerId, ct);
        if (cart is null)
        {
            cart = new Cart { CustomerId = customerId, IsActive = true };
            await _cartRepo.AddAsync(cart, ct);
            cart = await _cartRepo.GetActiveCartWithItemsAsync(customerId, ct) ?? cart;
        }
        return cart;
    }
}

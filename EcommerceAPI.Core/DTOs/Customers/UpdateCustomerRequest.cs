using System;

namespace EcommerceAPI.Core.DTOs.Customers;

public class UpdateCustomerRequest
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName  { get; set; } = null!;
    public string Email     { get; set; } = null!;
    public string? Phone    { get; set; }
}

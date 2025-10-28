using EcommerceAPI.Core.DTOs.Common;

namespace EcommerceAPI.Core.DTOs.Accounts;

public class RegisterRequest
{
    public string FirstName { get; set; } = null!;
    public string LastName  { get; set; } = null!;
    public string Email     { get; set; } = null!;
    public string Password  { get; set; } = null!;
    public string? Phone    { get; set; }

    public AddressDto? Address { get; set; }
}

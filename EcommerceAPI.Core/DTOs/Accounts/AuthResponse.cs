using System;

namespace EcommerceAPI.Core.DTOs.Accounts;

public class AuthResponse
{
    public string AccessToken { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }
}

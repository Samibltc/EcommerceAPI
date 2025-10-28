using System;

namespace EcommerceAPI.Core.DTOs.Accounts;

public class RegisterResponse
{
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    public string Message { get; set; } = "Registration successful. Please check your email to confirm.";
}

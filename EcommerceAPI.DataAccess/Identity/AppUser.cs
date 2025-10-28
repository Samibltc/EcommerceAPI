using System;
using EcommerceAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.DataAccess.Identity;

public class AppUser : IdentityUser<Guid>
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}

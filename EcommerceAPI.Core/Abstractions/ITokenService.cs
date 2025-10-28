using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

namespace EcommerceAPI.Core.Abstractions
{
    public interface ITokenService
    {
        string CreateToken(IEnumerable<Claim> claims, DateTime expiresAtUtc);
    }
}

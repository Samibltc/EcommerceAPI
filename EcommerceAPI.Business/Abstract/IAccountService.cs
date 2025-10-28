using System;
using System.Threading;
using System.Threading.Tasks;
using EcommerceAPI.Core.DTOs.Accounts;

namespace EcommerceAPI.Business.Abstract
{
    public interface IAccountService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
        Task<bool> ConfirmEmailAsync(Guid userId, string token, CancellationToken ct = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
        Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
        Task DeleteAccountAsync(Guid userId, CancellationToken ct = default);
    }
}

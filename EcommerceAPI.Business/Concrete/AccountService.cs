using System.Text;
using System.Text.Encodings.Web;
using AutoMapper;
using EcommerceAPI.Business.Abstract;
using EcommerceAPI.Core.Abstractions;
using EcommerceAPI.Core.DTOs.Accounts;
using EcommerceAPI.DataAccess.Abstract;
using EcommerceAPI.DataAccess.Identity;
using EcommerceAPI.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EcommerceAPI.Business.Concrete;

public class AccountService : IAccountService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ICustomerRepository _customerRepo;
    private readonly IEmailSender _emailSender;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ICustomerRepository customerRepo,
        IEmailSender emailSender,
        ITokenService tokenService,
        IMapper mapper,
        IConfiguration config,
        ILogger<AccountService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _customerRepo = customerRepo;
        _emailSender = emailSender;
        _tokenService = tokenService;
        _mapper = mapper;
        _config = config;
        _logger = logger;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        // Create domain customer first
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address is null ? null : new Entities.ValueObjects.Address
            {
                AddressLine1 = request.Address.AddressLine1,
                AddressLine2 = request.Address.AddressLine2,
                City = request.Address.City,
                StateOrRegion = request.Address.StateOrRegion,
                PostalCode = request.Address.PostalCode,
                Country = request.Address.Country
            }
        };

        await _customerRepo.AddAsync(customer, ct);
        _logger.LogInformation("Customer created. CustomerId={CustomerId}, Email={Email}", customer.Id, customer.Email);

        // Create identity user linked to customer
        var user = new AppUser
        {
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.Phone,
            EmailConfirmed = false,
            CustomerId = customer.Id
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var msg = string.Join("; ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
            throw new InvalidOperationException($"Registration failed: {msg}");
        }

        _logger.LogInformation("Identity user created. UserId={UserId}, LinkedCustomerId={CustomerId}, Email={Email}",
            user.Id, user.CustomerId, user.Email);

        // Email confirmation
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var appBaseUrl = _config["ClientBaseUrl"] ?? _config["AppBaseUrl"] ?? "https://localhost";
        var confirmUrl = $"{appBaseUrl}/api/accounts/confirm-email?userId={user.Id}&token={tokenEncoded}";

        // Plain log (copy/paste friendly)
        _logger.LogInformation("EmailConfirm URL: {Url}", confirmUrl);

        await _emailSender.SendAsync(user.Email!, "Confirm your email",
            $"Please confirm your account by clicking <a href=\"{HtmlEncoder.Default.Encode(confirmUrl)}\">here</a>.", ct);

        // Return IDs so caller can confirm using the exact userId
        return new RegisterResponse
        {
            UserId = user.Id,
            CustomerId = customer.Id,
            Message = "Registration successful. Please check your email to confirm."
        };
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string token, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new KeyNotFoundException("User not found.");

        var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userManager.ConfirmEmailAsync(user, decoded);
        return result.Succeeded;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
                   ?? throw new KeyNotFoundException("Invalid credentials.");

        var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!check.Succeeded) throw new KeyNotFoundException("Invalid credentials.");

        if (!user.EmailConfirmed)
            throw new InvalidOperationException("Email is not confirmed.");

        var claims = new[]
        {
            new System.Security.Claims.Claim("sub", user.Id.ToString()),
            new System.Security.Claims.Claim("email", user.Email ?? string.Empty),
            new System.Security.Claims.Claim("cid", user.CustomerId.ToString())
        };

        var lifetimeMinutes = int.TryParse(_config["Jwt:TokenLifetimeMinutes"], out var m) ? m : 60;
        var expiresAt = DateTime.UtcNow.AddMinutes(lifetimeMinutes);
        var token = _tokenService.CreateToken(claims, expiresAt);

        return new AuthResponse { AccessToken = token, ExpiresAtUtc = expiresAt };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null) return; // do not reveal existence

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var appBaseUrl = _config["ClientBaseUrl"] ?? _config["AppBaseUrl"] ?? "https://localhost";
        var resetUrl = $"{appBaseUrl}/api/accounts/reset-password?email={Uri.EscapeDataString(user.Email!)}&token={tokenEncoded}";

        await _emailSender.SendAsync(user.Email!, "Reset your password",
            $"Use the following link to reset your password: <a href=\"{resetUrl}\">Reset Password</a>", ct);
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
                   ?? throw new KeyNotFoundException("User not found.");

        var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
        var result = await _userManager.ResetPasswordAsync(user, decoded, request.NewPassword);
        return result.Succeeded;
    }

    // New: delete account safely (delete Identity user first, then linked Customer)
    public async Task DeleteAccountAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString())
                   ?? throw new KeyNotFoundException("User not found.");

        var customerId = user.CustomerId;

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var msg = string.Join("; ", result.Errors.Select(e => $"{e.Code}:{e.Description}"));
            throw new InvalidOperationException($"Failed to delete user: {msg}");
        }

        var customer = await _customerRepo.GetByIdAsync(customerId, ct);
        if (customer is not null)
        {
            await _customerRepo.DeleteAsync(customer, ct);
        }

        _logger.LogInformation("Deleted account. UserId={UserId}, CustomerId={CustomerId}", userId, customerId);
    }
}

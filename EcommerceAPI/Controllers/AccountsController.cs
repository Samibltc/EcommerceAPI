using System.Security.Claims;
using EcommerceAPI.Business.Abstract;
using EcommerceAPI.Core.DTOs.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _service;

    public AccountsController(IAccountService service) => _service = service;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var resp = await _service.RegisterAsync(request, ct);
        return Accepted(resp);
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token, CancellationToken ct)
    {
        var ok = await _service.ConfirmEmailAsync(userId, token, ct);
        return ok ? Ok(new { message = "Email confirmed." }) : BadRequest(new { message = "Invalid or expired token." });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await _service.LoginAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        await _service.ForgotPasswordAsync(request, ct);
        return Accepted(new { message = "If the email exists, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        var ok = await _service.ResetPasswordAsync(request, ct);
        return ok ? Ok(new { message = "Password reset successful." }) : BadRequest(new { message = "Invalid token." });
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteAccount(Guid userId, CancellationToken ct)
    {
        await _service.DeleteAccountAsync(userId, ct);
        return NoContent();
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("me")]
    public async Task<IActionResult> DeleteMyAccount(CancellationToken ct)
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(idStr, out var userId)) return Unauthorized();
        await _service.DeleteAccountAsync(userId, ct);
        return NoContent();
    }
}
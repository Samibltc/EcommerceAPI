using EcommerceAPI.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace EcommerceAPI.Services.Email;

public class DebugEmailSender : IEmailSender
{
    private readonly ILogger<DebugEmailSender> _logger;

    public DebugEmailSender(ILogger<DebugEmailSender> logger) => _logger = logger;

    public Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        _logger.LogInformation("Sending email to {Email}. Subject: {Subject}. Body: {Body}", toEmail, subject, htmlBody);
        return Task.CompletedTask;
    }
}

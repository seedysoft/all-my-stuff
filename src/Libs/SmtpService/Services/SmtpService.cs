using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Seedysoft.Libs.SmtpService.Services;

public sealed class SmtpService(IServiceProvider serviceProvider, IConfiguration configuration)
{
    private readonly Settings.SmtpServiceSettings SmtpServiceSettings = configuration
        .GetSection(nameof(Settings.SmtpServiceSettings)).Get<Settings.SmtpServiceSettings>()!;

    private readonly ILogger<SmtpService> Logger = serviceProvider.GetRequiredService<ILogger<SmtpService>>();

    public async Task SendMailAsync(
        string? to,
        string subject,
        string body,
        bool? isBodyHtml,
        CancellationToken cancellationToken = default!)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subject, nameof(subject));
        ArgumentException.ThrowIfNullOrWhiteSpace(body, nameof(body));

        bool IsHtmlBody = isBodyHtml ?? false;
        using System.Net.Mail.MailMessage Message = new()
        {
            From = new System.Net.Mail.MailAddress(SmtpServiceSettings.Username),
            Subject = subject,
            Body = IsHtmlBody ? body.ReplaceLineEndings("<br>") : body,
            IsBodyHtml = IsHtmlBody,
        };

        string[]? PossibleDirections = to?
            .Replace(',', ';')
            .Split(";", StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);

        for (int i = 0; i < PossibleDirections?.Length; i++)
        {
            if (System.Net.Mail.MailAddress.TryCreate(PossibleDirections[i], out System.Net.Mail.MailAddress? m))
                Message.Bcc.Add(m);
        }

        if (!Message.Bcc.Any())
            Message.To.Add(Message.From);

        await SendAsync(Message, cancellationToken);
    }

    private async Task SendAsync(
        System.Net.Mail.MailMessage message,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        System.Net.Mail.SmtpClient MailSender = new()
        {
            Host = SmtpServiceSettings.Host,
            Port = SmtpServiceSettings.Port,
            Credentials = new System.Net.NetworkCredential(SmtpServiceSettings.Username, SmtpServiceSettings.Password),
            EnableSsl = true,
            UseDefaultCredentials = false,
        };

        if (System.Diagnostics.Debugger.IsAttached)
            await Task.CompletedTask;
        else
            await MailSender.SendMailAsync(message, cancellationToken);

        Logger.LogInformation("Message {message} sent.", message);
    }
}

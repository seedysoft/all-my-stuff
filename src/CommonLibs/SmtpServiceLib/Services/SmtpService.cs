using Microsoft.Extensions.Logging;

namespace Seedysoft.SmtpServiceLib.Services;

public sealed class SmtpService
{
    private readonly Settings.SmtpServiceSettings SmtpServiceSettings;
    private readonly ILogger<SmtpService> Logger;

    public SmtpService(
        Settings.SmtpServiceSettings smtpServiceSettings,
        ILogger<SmtpService> logger)
    {
        SmtpServiceSettings = smtpServiceSettings ?? throw new ArgumentNullException(nameof(smtpServiceSettings));
        Logger = logger;
    }

    public async Task SendMailAsync(
        string? to,
        string subject,
        string body,
        bool? isBodyHtml,
        CancellationToken cancellationToken = default!)
    {
        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException($"'{nameof(subject)}' cannot be null or whitespace", nameof(subject));
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException($"'{nameof(body)}' cannot be null or whitespace", nameof(body));

        bool IsHtmlBody = isBodyHtml ?? false;
        using System.Net.Mail.MailMessage Message = new()
        {
            From = new System.Net.Mail.MailAddress(SmtpServiceSettings.Username),
            Subject = subject,
            Body = IsHtmlBody ? body.ReplaceLineEndings("<br>") : body,
            IsBodyHtml = IsHtmlBody,
        };

        string[]? PossibleDirections = to?
            .Replace(",", ";")
            .Split(";", StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);

        for (int i = 0; i < PossibleDirections?.Length; i++)
        {
            if (System.Net.Mail.MailAddress.TryCreate(PossibleDirections[i], out System.Net.Mail.MailAddress? m))
                Message.Bcc.Add(m);
        }

        if (!Message.Bcc.Any())
            Message.To.Add(Message.From);

        await SendAsync(Message, SmtpServiceSettings, cancellationToken);
    }

    private async Task SendAsync(
        System.Net.Mail.MailMessage message,
        Settings.SmtpServiceSettings smtpServiceSettings,
        CancellationToken cancellationToken)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        if (smtpServiceSettings is null)
            throw new ArgumentNullException(nameof(smtpServiceSettings));

        System.Net.Mail.SmtpClient MailSender = new()
        {
            Host = smtpServiceSettings.Host,
            Port = smtpServiceSettings.Port,
            Credentials = new System.Net.NetworkCredential(smtpServiceSettings.Username, smtpServiceSettings.Password),
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

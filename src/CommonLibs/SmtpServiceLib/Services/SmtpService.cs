using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Seedysoft.SmtpServiceLib.Services;

public class SmtpService
{
    private static bool isConfigured;

    public static void Configure(IHostBuilder hostBuilder)
    {
        if (!isConfigured)
        {
            _ = hostBuilder
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) => ConfigJsonFile(configurationBuilder, hostBuilderContext.HostingEnvironment))

                .ConfigureServices((hostBuilderContext, services) => ConfigServices(services, hostBuilderContext.Configuration));

            isConfigured = true;
        }
    }
    public static void Configure(IConfigurationBuilder configurationBuilder, IServiceCollection services, IConfiguration configuration, IHostEnvironment hostEnvironment)
    {
        if (!isConfigured)
        {
            ConfigJsonFile(configurationBuilder, hostEnvironment);

            ConfigServices(services, configuration);

            isConfigured = true;
        }
    }
    private static void ConfigJsonFile(IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment) =>
        _ = configurationBuilder.AddJsonFile("appsettings.SmtpServiceSettings.json", false, true);
    private static void ConfigServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddSingleton(configuration.GetSection(nameof(Settings.SmtpServiceSettings)).Get<Settings.SmtpServiceSettings>()!);

        services.TryAddScoped<SmtpService>();
    }

    private readonly Settings.SmtpServiceSettings SmtpServiceSettings;

    public SmtpService(Settings.SmtpServiceSettings smtpServiceSettings) =>
      SmtpServiceSettings = smtpServiceSettings ?? throw new ArgumentNullException(nameof(smtpServiceSettings));

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

    private static async Task SendAsync(
        System.Net.Mail.MailMessage message
        , Settings.SmtpServiceSettings smtpServiceSettings
        , CancellationToken cancellationToken)
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
        };

        await MailSender.SendMailAsync(message, cancellationToken);
    }
}

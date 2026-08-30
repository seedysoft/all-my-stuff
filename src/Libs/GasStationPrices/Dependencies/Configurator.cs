using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Seedysoft.Libs.GasStationPrices.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.{nameof(Settings.GasStationPricesSettings)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Settings.GasStationPricesSettings)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
    }

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddSingleton<Services.GasStationPricesService>();

        _ = hostApplicationBuilder.Services.AddHttpClient(name: nameof(GasStationPrices))
            .ConfigureHttpClient(static (serviceProvider, httpClient) =>
            {
                Services.GasStationPricesService gasStationPricesService = serviceProvider.GetRequiredService<Services.GasStationPricesService>();

                httpClient.BaseAddress = new Uri(gasStationPricesService.GasStationPricesSettings.Minetur.Urls.Base);

                //httpClient.DefaultRequestHeaders.Accept.Clear();
                //httpClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");
                //httpClient.DefaultRequestHeaders.AcceptEncoding.Clear();
                //httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
                //httpClient.DefaultRequestHeaders.Connection.Clear();
                //httpClient.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
                //httpClient.DefaultRequestHeaders.UserAgent.Clear();
                //httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{nameof(GasStationPrices)}/1.0 (Windows 10; Contact: seedysoft@gmail.com)");

            }) // ConfigureHttpClient

            .SetHandlerLifetime(Timeout.InfiniteTimeSpan)                       // Disable rotation, as it is handled by PooledConnectionLifetime

            .UseSocketsHttpHandler(static (handler, _) =>
            {
                //handler.Expect100ContinueTimeout = TimeSpan.FromMinutes(1);

                // SSL-Session:
                //     Protocol  : TLSv1.2
                //     Cipher    : AES256-GCM-SHA384
                //     Session-ID: AA420000B5D101C63CC797CA66CB2515E8989D461A99639689D539B3470C4C5A
                //     Session-ID-ctx:
                //     Master-Key: 97B804C20A413B37ED663206B41236ABE9EE901F6FF50AC8F21A63E929C81D61712A3A8A029467313B9939DD54911BEB
                //     PSK identity: None
                //     PSK identity hint: None
                //     SRP username: None
                //     Start Time: 1787518001
                //     Timeout   : 7200 (sec)
                //     Verify return code: 0 (ok)
                //     Extended master secret: yes

                handler.PooledConnectionLifetime = TimeSpan.FromMinutes(2);     // Recreate connection every 2 minutes

                // handler.SslOptions.AllowRenegotiation = true;                  // true by default
                //if (!OperatingSystem.IsWindows())                               // 'CipherSuitesPolicy' is unsupported on: 'windows'.
                //{
                //    handler.SslOptions.CipherSuitesPolicy = new([
                //        System.Net.Security.TlsCipherSuite.TLS_RSA_WITH_AES_256_GCM_SHA384,
                //        System.Net.Security.TlsCipherSuite.TLS_RSA_WITH_AES_128_GCM_SHA256,
                //        System.Net.Security.TlsCipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA256,
                //        System.Net.Security.TlsCipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA256,
                //        System.Net.Security.TlsCipherSuite.TLS_RSA_WITH_AES_256_CBC_SHA,
                //        System.Net.Security.TlsCipherSuite.TLS_RSA_WITH_AES_128_CBC_SHA,
                //    ]);
                //}

                handler.SslOptions.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;

                //handler.SslOptions.LocalCertificateSelectionCallback = new System.Net.Security.LocalCertificateSelectionCallback(
                //    static (sender, target, local, remote, accept) =>
                //    {
                //        System.Security.Cryptography.X509Certificates.X509Certificate? x509Certificate;
                //        string runtimeIdentifier = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier;
                //        switch (runtimeIdentifier)
                //        {
                //            case Core.Constants.SupportedRuntimeIdentifiers.LinuxArm64:
                //                //case Core.Constants.SupportedRuntimeIdentifiers.LinuxX64:
                //                System.Security.Cryptography.X509Certificates.X509Certificate2Collection certificates = [];
                //                certificates.ImportFromPemFile("/etc/ssl/certs/SeedySoft_Root_CA.pem"); // SeedySoft_Root_CA.pem    raspberrypi4
                //                return certificates.First();

                //            //case Core.Constants.SupportedRuntimeIdentifiers.WinX64:
                //            //System.Security.Cryptography.X509Certificates.X509Store x509Store = new(
                //            //    storeName: System.Security.Cryptography.X509Certificates.StoreName.My,
                //            //    storeLocation: System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
                //            //    flags: System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);

                //            //if (!x509Store.IsOpen)
                //            //    x509Store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);

                //            //x509Certificate = x509Store.Certificates[0];

                //            //if (x509Store.IsOpen)
                //            //    x509Store.Close();

                //            //_ = local.Add(x509Certificate);
                //            //    return   System.Security.Cryptography.X509Certificates.X509CertificateLoader.LoadCertificateFromFile("C:\\Users\\amt\\Downloads\\swift\\swift.pem");

                //            default:
                //                x509Certificate = null;
                //                //    if (Logger.IsEnabled(LogLevel.Error))
                //                //        Logger.LogError($"RuntimeIdentifier {runtimeIdentifier} not supported");
                //                break;
                //        }

                //        return x509Certificate;
                //    });

                //handler.SslOptions.RemoteCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(
                //    static (sender, cert, chain, errors) =>
                //    {
                //        if (errors == System.Net.Security.SslPolicyErrors.None)
                //            return true; // No errors: trust

                //        // Serial Number of the trusted self-signed certificate (get from cert details)
                //        const string trustedSerialNumber = "2fae6b0b22cd797362481c6120e293cf";

                //        return cert?.GetSerialNumberString()?.Equals(trustedSerialNumber, StringComparison.OrdinalIgnoreCase) ?? false;
                //    });

            }) // UseSocketsHttpHandler
        ;
    }
}

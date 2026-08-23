using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Seedysoft.Libs.Travel.Dependencies;

public sealed class Configurator : Core.Dependencies.ConfiguratorBase
{
    protected override void AddJsonFiles(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        string CurrentEnvironmentName = hostApplicationBuilder.Environment.EnvironmentName;

        _ = hostApplicationBuilder.Configuration
            .AddJsonFile($"appsettings.{nameof(Settings.TravelSettings)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{nameof(Settings.TravelSettings)}.{CurrentEnvironmentName}.json", optional: false, reloadOnChange: true);
        ;
    }

    protected override void AddDbContexts(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder) { /* No DbContexts */ }

    protected override void AddMyServices(Microsoft.Extensions.Hosting.IHostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.TryAddScoped<Services.Geocoding.GeocodingService>();
        hostApplicationBuilder.Services.TryAddScoped<Services.Routing.RoutingService>();

        _ = hostApplicationBuilder.Services.AddHttpClient(name: nameof(Travel))
            //.ConfigureHttpClient(static configureClient =>
            //{
            //    configureClient.DefaultRequestHeaders.Accept.Clear();
            //    configureClient.DefaultRequestHeaders.Accept.ParseAdd("*/*");
            //    configureClient.DefaultRequestHeaders.AcceptEncoding.Clear();
            //    configureClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            //    configureClient.DefaultRequestHeaders.Connection.Clear();
            //    configureClient.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");
            //    configureClient.DefaultRequestHeaders.UserAgent.Clear();
            //    configureClient.DefaultRequestHeaders.UserAgent.ParseAdd($"{nameof(Travel)}/1.0 (Windows 10; Contact: seedysoft@gmail.com)");
            //})
            //.ConfigurePrimaryHttpMessageHandler(static () =>
            //{
            //    SocketsHttpHandler handler = new()
            //    {
            //        SslOptions = new System.Net.Security.SslClientAuthenticationOptions()
            //        {
            //            // SslProtocols.None lets the OS choose the best protocol (recommended)
            //            EnabledSslProtocols = System.Security.Authentication.SslProtocols.None,

            //            LocalCertificateSelectionCallback = new System.Net.Security.LocalCertificateSelectionCallback(
            //                static (sender, target, local, remote, accept) =>
            //                {
            //                    System.Security.Cryptography.X509Certificates.X509Certificate? x509Certificate;
            //                    x509Certificate = System.Security.Cryptography.X509Certificates.X509CertificateLoader.LoadCertificateFromFile("C:\\Users\\amt\\Downloads\\swift\\swift.pem");

            //                    System.Security.Cryptography.X509Certificates.X509Store x509Store = new(
            //                        storeName: System.Security.Cryptography.X509Certificates.StoreName.My,
            //                        storeLocation: System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            //                        flags: System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);

            //                    if (!x509Store.IsOpen)
            //                        x509Store.Open(System.Security.Cryptography.X509Certificates.OpenFlags.ReadOnly);

            //                    x509Certificate = x509Store.Certificates[0];

            //                    if (x509Store.IsOpen)
            //                        x509Store.Close();

            //                    return x509Certificate;
            //                }),

            //            RemoteCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(
            //                static (sender, cert, chain, errors) =>
            //                {
            //                    if (errors == System.Net.Security.SslPolicyErrors.None)
            //                        return true; // No errors: trust

            //                    // Serial Number of the trusted self-signed certificate (get from cert details)
            //                    const string trustedSerialNumber = "2fae6b0b22cd797362481c6120e293cf";

            //                    return cert?.GetSerialNumberString()?.Equals(trustedSerialNumber, StringComparison.OrdinalIgnoreCase) ?? false;
            //                }),
            //        },
            //    };
            //    //HttpClientHandler handler = new()
            //    //{
            //    //    AllowAutoRedirect = true,
            //    //    ClientCertificateOptions = ClientCertificateOption.Automatic,//.Manual,
            //    //    PreAuthenticate = true,
            //    //    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            //    //    SslProtocols = System.Security.Authentication.SslProtocols.None,// System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13,
            //    //};
            //    ////_ = handler.ClientCertificates.Add(clientCertificate);

            //    return handler;
            //})
            ;
    }
}

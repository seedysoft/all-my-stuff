using Seedysoft.Libs.Infrastructure.Extensions;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Seedysoft.BlazorWebApp.Server;

public class Program : Libs.Core.ProgramBase
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        await ObtainCommandLineAsync(args);

        WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

        _ = webApplicationBuilder.AddAllMyDependencies();

        // TODO         Learn how it works
        _ = webApplicationBuilder.Configuration.AddInMemoryCollection(Libs.Core.Models.Config.RuntimeSettings.GetValues(Settings));

        // Add services to the container.
        _ = webApplicationBuilder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        _ = webApplicationBuilder.Services.ConfigureHttpJsonOptions(static configureOptions =>
        {
            configureOptions.SerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            configureOptions.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            configureOptions.SerializerOptions.ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip;
        });

        /*
         * It would be great if the code below could be accomplished by configuration. I do not know how. Now it is hard-coded.
         */
        _ = webApplicationBuilder.WebHost.ConfigureKestrel(static kestrelServerOptions =>
        {
            kestrelServerOptions.ConfigureHttpsDefaults(static httpsConnectionAdapterOptions =>
            {
                /*
                    If we want to check or customize client-certificate validation we can use the callback below.
                */
                //httpsConnectionAdapterOptions.ClientCertificateValidation = (certificate, chain, sslPolicyErrors) =>
                //{
                //	return sslPolicyErrors == SslPolicyErrors.None;
                //};

                //httpsConnectionAdapterOptions.OnAuthenticate = static (_, sslServerAuthenticationOptions) =>
                //{
                //    if (!sslServerAuthenticationOptions.ClientCertificateRequired)
                //        return;

                //    if (sslServerAuthenticationOptions.ServerCertificate is not X509Certificate2 serverCertificate)
                //        throw new InvalidOperationException("The server-certificate is invalid.");

                //    sslServerAuthenticationOptions.CertificateChainPolicy = new X509ChainPolicy
                //    {
                //        /*
                //            RevocationMode = X509RevocationMode.NoCheck
                //            We only set this value because we are using certificates created by ourselves. The default value for RevocationMode is X509RevocationMode.Online
                //            and that value should be used when using "real" certificates. If we use X509RevocationMode.Online when using the certificates in this solution we
                //            will get client-certificate validation errors.
                //        */
                //        RevocationMode = X509RevocationMode.NoCheck,
                //        /*
                //            TrustMode = X509ChainTrustMode.System
                //            We use the system for trust. As we have imported the intermediate- and root-certificates to the container it should work. The other value
                //            is X509ChainTrustMode.CustomRootTrust. I do not know what is required to do when using that value. But using the system-trust feels more safe.
                //        */
                //        TrustMode = X509ChainTrustMode.System
                //    };

                //    SslCertificateTrust? sslCertificateTrust;
                //    if (OperatingSystem.IsWindows())
                //    {
                //        /*
                //            On Windows we can only set the SSL-certificate-trust to the LocalMachine store. If not we get an exception.

                //            System.PlatformNotSupportedException: 'Only LocalMachine stores are supported on Windows.':

                //            #if TARGET_WINDOWS
                //                if (sendTrustInHandshake && store.Location != StoreLocation.LocalMachine)
                //                {
                //                    throw new PlatformNotSupportedException(SR.net_ssl_trust_store);
                //                }
                //            #endif
                //        */
                //        using var store = new X509Store("Store-bc8fd192-bb7a-41a1-b470-b2c356aac15b", StoreLocation.LocalMachine);
                //        // This store must have been set up in the Windows Certificate Manager. You can set it upp with .windows-certificate-management/Setup in this solution.
                //        store.Open(OpenFlags.ReadOnly);

                //        sslCertificateTrust = SslCertificateTrust.CreateForX509Store(store, true);
                //    }
                //    else
                //    {
                //        var certificates = new X509Certificate2Collection();
                //        certificates.ImportFromPemFile("/etc/ssl/certs/intermediate-1.crt");
                //        certificates.ImportFromPemFile("/etc/ssl/certs/intermediate-2.crt");

                //        sslCertificateTrust = SslCertificateTrust.CreateForX509Collection(certificates, true);
                //    }

                //    sslServerAuthenticationOptions.ServerCertificateContext = SslStreamCertificateContext.Create(serverCertificate, null, false, sslCertificateTrust);
                //};
            });
            /*
                The commented code below is another way to set the trust list. But then we have to build the whole SslServerAuthenticationOptions-instance and the
                configuration from appsettings.json will not work.
            */
            //kestrelServerOptions.ConfigureEndpointDefaults(listenOptions =>
            //{
            //	listenOptions.UseHttps((sslStream, sslClientHelloInfo, state, cancellationToken) =>
            //	{
            //		var sslServerAuthenticationOptions = new SslServerAuthenticationOptions();

            //		// ... build the SslServerAuthenticationOptions-instance as you like.

            //		return new ValueTask<SslServerAuthenticationOptions>(sslServerAuthenticationOptions);
            //	}, null);
            //});
        });

        WebApplication webApplication = webApplicationBuilder.Build();

        // Configure the HTTP request pipeline.
        if (webApplication.Environment.IsDevelopment())
            webApplication.UseWebAssemblyDebugging();
        else
            _ = webApplication.UseExceptionHandler("/Error");

        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/security-considerations?view=aspnetcore-10.0#security-checklist
        _ = webApplication.UseHostFiltering();      // Validate Host header
        _ = webApplication.UseHttpsRedirection();   // Redirect HTTP -> HTTPS
        _ = webApplication.UseHsts();               // Add Strict-Transport-Security
        _ = webApplication.UseRateLimiter();        // Rate limiting
        //_ = webApplication.UseCors();               // CORS (if needed)
        //_ = webApplication.UseAuthentication();
        //_ = webApplication.UseAuthorization();

        _ = webApplication
            .UseAntiforgery()
            .UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

        _ = webApplication.MapStaticAssets();

        _ = webApplication.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies([
                typeof(Libs.MapRazorClassLibrary.MapComponent).Assembly,
                typeof(Client._Imports).Assembly,
            ]);

        await webApplication.RunAsync();
    }
}

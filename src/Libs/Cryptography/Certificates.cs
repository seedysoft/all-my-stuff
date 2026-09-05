namespace Seedysoft.Libs.Cryptography;

public class Program
{
    public static async Task Main()
    {
        try
        {
            // 1. Generate RSA key pair
            Org.BouncyCastle.Crypto.Generators.RsaKeyPairGenerator keyGen = new();
            keyGen.Init(new Org.BouncyCastle.Crypto.KeyGenerationParameters(new Org.BouncyCastle.Security.SecureRandom(), 2048));
            Org.BouncyCastle.Crypto.AsymmetricCipherKeyPair keyPair = keyGen.GenerateKeyPair();

            // 2. Create certificate generator
            Org.BouncyCastle.X509.X509V3CertificateGenerator certGen = new();
            Org.BouncyCastle.Asn1.X509.X509Name certName = new("CN=Test Client");
            var serialNo = Org.BouncyCastle.Math.BigInteger.ProbablePrime(120, new Random());

            certGen.SetSerialNumber(serialNo);
            certGen.SetIssuerDN(certName);
            certGen.SetSubjectDN(certName);
            certGen.SetNotBefore(DateTime.UtcNow.AddDays(-1));
            certGen.SetNotAfter(DateTime.UtcNow.AddYears(1));
            certGen.SetPublicKey(keyPair.Public);

            // 3. Create signature factory (replaces SetSignatureAlgorithm)
            Org.BouncyCastle.Crypto.Operators.Asn1SignatureFactory signatureFactory = new("SHA256WITHRSA", keyPair.Private);

            // 4. Generate certificate
            Org.BouncyCastle.X509.X509Certificate cert = certGen.Generate(signatureFactory);

            // 5. Create PKCS#12 store
            Org.BouncyCastle.Pkcs.Pkcs12Store store = new Org.BouncyCastle.Pkcs.Pkcs12StoreBuilder().Build();
            string friendlyName = cert.SubjectDN.ToString();
            Org.BouncyCastle.Pkcs.X509CertificateEntry certEntry = new(cert);
            store.SetCertificateEntry(friendlyName, certEntry);
            store.SetKeyEntry(friendlyName, new Org.BouncyCastle.Pkcs.AsymmetricKeyEntry(keyPair.Private), [certEntry]);

            // 6. Save to memory and load into X509Certificate2
            using System.IO.MemoryStream ms = new();
            store.Save(ms, "password".ToCharArray(), new Org.BouncyCastle.Security.SecureRandom());

            System.Security.Cryptography.X509Certificates.X509Certificate2 clientCert =
                System.Security.Cryptography.X509Certificates.X509CertificateLoader.LoadPkcs12(ms.ToArray(), "password");

            // 7. Use HttpClient with client certificate
            HttpClientHandler handler = new();
            _ = handler.ClientCertificates.Add(clientCert);
            handler.ServerCertificateCustomValidationCallback = (msg, cert2, chain, errors) => true; // Accept all for demo

            using HttpClient client = new(handler);
            HttpResponseMessage response = await client.GetAsync("https://postman-echo.com/get");
            string content = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Response:");
            Console.WriteLine(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }
}

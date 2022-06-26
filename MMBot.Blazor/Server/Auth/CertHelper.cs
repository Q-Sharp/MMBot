using Microsoft.IdentityModel.Tokens;

namespace MMBot.Blazor.Server.Auth;

public static class CertHelpers
{
    private static X509Certificate2 GetCertificateFromStore(string certName)
    {
        var store = new X509Store(StoreLocation.LocalMachine);
        try
        {
            store.Open(OpenFlags.ReadOnly);

            var certCollection = store.Certificates;

            var currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);

            return currentCerts.FirstOrDefault(x => x.Subject.Contains(certName) || x.FriendlyName.Contains(certName));
        }
        finally
        {
            store.Close();
        }
    }

    public static X509SecurityKey GetSecKeyFromCertStore(string certName)
        => new(GetCertificateFromStore(certName));
}

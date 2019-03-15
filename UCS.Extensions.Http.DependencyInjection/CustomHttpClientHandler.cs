using System.Net;
using System.Net.Http;

namespace UCS.Extensions.Http.DependencyInjection
{
    public class CustomHttpClientHandler : HttpClientHandler
    {
        public void AddCredentials(string login, string password)
        {
            Credentials = new NetworkCredential(login, password);
        }

        public void DisableProxy()
        {
            UseProxy = false;
        }

        //не проводить валидацию доверенных сертификатов сервера
        public void SkipServerCertificateValidation()
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        }

        //DecompressionMethods.Deflate | DecompressionMethods.GZip |  DecompressionMethods.None[default]
        public void SetDecompression(DecompressionMethods methods)
        {
            AutomaticDecompression = methods;
        }
    }
}

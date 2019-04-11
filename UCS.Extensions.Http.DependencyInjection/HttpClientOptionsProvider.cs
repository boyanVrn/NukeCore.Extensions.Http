using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace UCS.Extensions.Http.DependencyInjection
{

    public class HttpClientOptionsProvider
    {
        private static readonly int def_timeout_sec = 60;

        public Uri BaseAddress { get; set; }
        public NetworkCredential Credentials { get; private set; }
        public TimeSpan Timeout { get; set; }

        public bool HasProxy { get; private set; }
        public bool HasServerCertificateValidation { get; private set; }
        public DecompressionMethods ResponseAutoDecompressionType { get; set; }

        public HttpClientOptionsProvider()
        {
            HasProxy = true;
            HasServerCertificateValidation = true;
            ResponseAutoDecompressionType = DecompressionMethods.None;
            Timeout = TimeSpan.FromSeconds(def_timeout_sec);
        }

        public List<MediaTypeWithQualityHeaderValue> DefaultRequestHeaders = new List<MediaTypeWithQualityHeaderValue>();

        public void AddCredentials(string login, string password)
        {
            Credentials = new NetworkCredential(login, password);
        }

        public void AddJsonDefaultRequestHeader()
        {
            DefaultRequestHeaders.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void AddXmlDefaultRequestHeader()
        {
            DefaultRequestHeaders.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        }

        public void DisableProxy()
        {
            HasProxy = false;
        }

        //DecompressionMethods.Deflate | DecompressionMethods.GZip |  DecompressionMethods.None[default]
        public void SetDecompression(DecompressionMethods methods)
        {
            ResponseAutoDecompressionType = methods;
        }

        //не проводить валидацию доверенных сертификатов сервера
        public void SkipServerCertificateValidation()
        {
            HasServerCertificateValidation = false;
            //ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        }

    }
}

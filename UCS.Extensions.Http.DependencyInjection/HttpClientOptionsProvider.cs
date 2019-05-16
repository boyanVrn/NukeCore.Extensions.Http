using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using UCS.Extensions.Http.Models.Additional;

namespace UCS.Extensions.Http.DependencyInjection
{

    public class HttpClientOptionsProvider
    {
        private static readonly int DEF_TIMEOUT_SEC = 60;
        private Uri _baseAddress;

        public Uri BaseAddress
        {
            get => _baseAddress;
            set => _baseAddress = value.AppendSlash();
        }

        public NetworkCredential Credentials { get; private set; }
        public TimeSpan Timeout { get; set; }

        public bool HasProxy { get; private set; }
        public bool HasServerCertificateValidation { get; private set; }
        public DecompressionMethods ResponseAutoDecompressionType { get; private set; }

        public HttpClientOptionsProvider()
        {
            HasProxy = true;
            HasServerCertificateValidation = true;
            ResponseAutoDecompressionType = DecompressionMethods.None;
            Timeout = TimeSpan.FromSeconds(DEF_TIMEOUT_SEC);

            AcceptHeaders = new List<MediaTypeWithQualityHeaderValue>();
            RequestHeaders = new CustomHttpHeaders();
        }

        public List<MediaTypeWithQualityHeaderValue> AcceptHeaders { get; }
        public CustomHttpHeaders RequestHeaders { get; }

        public void AddCredentials(string login, string password)
        {
            Credentials = new NetworkCredential(login, password);
        }

        public void AddCustomRequestHeader(string name, string value)
        {
            RequestHeaders.AddOrUpdate(name, value);
        }

        public void AddJsonDefaultAcceptHeader()
        {
            AcceptHeaders.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void AddXmlDefaultAcceptHeader()
        {
            AcceptHeaders.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
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

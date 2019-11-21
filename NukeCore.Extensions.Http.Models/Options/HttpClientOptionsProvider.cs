using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using NukeCore.Extensions.Http.Common.Helpers;
using NukeCore.Extensions.Http.Common.Models;

namespace NukeCore.Extensions.Http.Models.Options
{

    public class HttpClientOptionsProvider
    {

        //private const string CNT_CLIENT_ID = "x-http-client-uuid";
        //public Guid InstanceUuid { get; }

        #region Props
        private Uri _baseAddress;
        public Uri BaseAddress
        {
            get => _baseAddress;
            private set => _baseAddress = HttpClientHelper.AppendSlash(value);
        }
        public NetworkCredential Credentials { get; private set; }
        public bool HasProxy { get; private set; }
        public bool HasServerCertificateValidation { get; private set; }
        public DecompressionMethods ResponseAutoDecompressionType { get; private set; }
        public HttpSenderOptions SenderOptions { get; private set; }
        public List<MediaTypeWithQualityHeaderValue> AcceptHeaders { get; }
        public CustomHttpHeaders RequestHeaders { get; }

        #endregion

        public HttpClientOptionsProvider()
        {
           // InstanceUuid = Guid.NewGuid();
            HasProxy = true;
            HasServerCertificateValidation = true;
            ResponseAutoDecompressionType = DecompressionMethods.None;

            AcceptHeaders = new List<MediaTypeWithQualityHeaderValue>();
            RequestHeaders = new CustomHttpHeaders();
            //RequestHeaders.AddOrUpdate(CNT_CLIENT_ID, InstanceUuid.ToString());
            SenderOptions = new HttpSenderOptions();
        }

        #region Setters methods
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

        public void ConfigureSenderOptions(Action<HttpSenderOptions> senderOptAction)
        {
            var so = new HttpSenderOptions();
            senderOptAction.Invoke(so);

            SenderOptions = so;
        }

        public void SetBaseAddress(string uri)
        {
            if (string.IsNullOrEmpty(uri))
                throw new ArgumentNullException(nameof(uri), "Cannot set base address");

            BaseAddress = new Uri(uri);
        }

        public void SetBaseAddress(Uri uri) => BaseAddress = uri;
        #endregion
    }
}

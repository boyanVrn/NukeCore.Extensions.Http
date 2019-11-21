using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NukeCore.Extensions.Http.Common.Models;
using NukeCore.Extensions.Http.Models.Factory;
using NukeCore.Extensions.Http.Models.Options;
using NukeCore.Extensions.Http.Sender;
using NukeCore.Extensions.Http.Tests.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NukeCore.Extensions.Http.Tests
{
    public class HttpXmlUnitTest
    {
        private const string BaseAddress = "http://172.18.2.28:9192";

        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpSenderXml> _nullRepoLogger;
        private readonly ResponseFactory _responseFactory;

        private readonly ConcreteHttpSenderXml _xmlSender;
        private readonly HttpClientOptionsProvider _optionsProvider;
        private readonly IHttpSenderOptionsPool _optionsPool;

        public class ConcreteHttpSenderXml : HttpSenderXml
        {
            public ConcreteHttpSenderXml(HttpClient client, IHttpSenderOptionsPool optionsPool, IResponseFactory responseFactory, ILogger<HttpSenderXml> logger)
                : base(client, optionsPool.Take<ConcreteHttpSenderXml>(), responseFactory, logger) { }
        }

        public HttpXmlUnitTest()
        {
            if (string.IsNullOrEmpty(BaseAddress))
                throw new ArgumentNullException($"Empty test params");

            _optionsProvider = CreateHttpSenderOptions();
            _httpClient = CreateHttpClient(_optionsProvider.BaseAddress);
            _nullRepoLogger = new NullLoggerFactory().CreateLogger<HttpSenderXml>();
            _responseFactory = new ResponseFactory();
            _optionsPool = HttpSenderOptionsPool.CreateInstance<ConcreteHttpSenderXml>(_optionsProvider.SenderOptions);

            _xmlSender = new ConcreteHttpSenderXml(_httpClient, _optionsPool, _responseFactory, _nullRepoLogger);

        }

        private static HttpClient CreateHttpClient(Uri url)
        {
            var httpClientHandler = new HttpClientHandler
            {
                UseProxy = false
            };

            var res = new HttpClient(httpClientHandler)
            {
                BaseAddress = url,
                Timeout = Timeout.InfiniteTimeSpan
            };

            res.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            return res;
        }

        private static HttpClientOptionsProvider CreateHttpSenderOptions()
        {
            var provider = new HttpClientOptionsProvider();
            provider.SetBaseAddress(BaseAddress);

            provider.ConfigureSenderOptions(opt =>
            {
                opt.RequestTimeout = TimeSpan.FromSeconds(10);
                opt.ValidateErrorsInResponse = true;
                opt.XmlParseSettings = new XmlParseSettings
                {
                    Serialize = new XmlSerializeSettings()
                    {
                        RemoveNilElements = true
                    },

                    Deserialize = new XmlSerializeSettings()
                    {
                        RemoveEmptyElements = true,
                        RemoveNilElements = true
                    }
                };
            });

            return provider;
        }


        [Fact]
        public async Task http_send_xml_test()
        {
            try
            {
                var query = $@"
<?xml version=""1.0"" encoding=""utf-8"" standalone=""yes"" ?>
	<Message Action=""Get system time"" Terminal_Type=""999"" Global_Type=""ABC"" Unit_ID=""1"" User_ID=""1"">
</Message>";

                var result = await _xmlSender.SendHttpRequest<SystemTime>(HttpMethod.Post, null, query, CancellationToken.None);

                Assert.NotNull(result);
                Assert.True(result.IsSuccess);
            }
            catch (Exception e)
            {
                Assert.Null(e);
            }
        }
    }
}

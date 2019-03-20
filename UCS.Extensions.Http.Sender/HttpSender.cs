using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCS.Extensions.Http.Errors;
using UCS.Extensions.Http.Sender.Entities;

namespace UCS.Extensions.Http.Sender
{
    public class HttpSender : IHttpSender
    {
        public HttpClient Client { get; }
        private readonly ILogger _logger;

        public HttpSender(HttpClient client, ILogger logger)
        {
            _logger = logger;
            Client = client;
        }

        private HttpContent CreateContent<T>(T body, JsonSerializerSettings serializeSettings)
        {
            if (body == null) return null;

            if (body is byte[] bytes)
            {
                var content = new ByteArrayContent(bytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                return content;
            }
            else
            {
                var postedData = JsonConvert.SerializeObject(body, serializeSettings);

                if (!string.IsNullOrEmpty(postedData))
                    _logger.LogDebug($"PostedData: {postedData}");

                var content = new StringContent(postedData, Encoding.UTF8);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return content;
            }
        }

        protected virtual async Task CheckResponseStatusCode(HttpResponseMessage src)
        {
            if (src.StatusCode != HttpStatusCode.OK)
                throw new HttpExc(src.StatusCode, await HttpSenderHelper.ExtractBodyAsync(src.Content));
        }

        protected virtual bool CheckResponseBody(string src, out string errText)
        {
            errText = string.Empty;
            try
            {
                var jToken = JToken.Parse(src);

                if (!jToken.HasValues || jToken.First == null) return false;

                errText = jToken["error"]?.Value<string>();
                return !string.IsNullOrEmpty(errText);
            }
            catch (JsonReaderException)
            {
                errText = "Json parse error";
                return false;
            }
        }

        public async Task SendHttpRequest(HttpMethod requestType, string apiMethod, string postedData,
            bool validateResponse = false, CancellationToken cancel = default)
                => await SendHttpRequest<string, object>(requestType, apiMethod, string.Empty,
                    (opt, headers) => { opt.ValidateErrorsInResponse = validateResponse; }, cancel);

        public async Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, string postedData,
            bool validateResponse = false, CancellationToken cancel = default) where T : class
                => await SendHttpRequest<string, T>(requestType, apiMethod, postedData, 
                    (opt, headers) => { opt.ValidateErrorsInResponse = validateResponse; }, cancel);

        public async Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod,
            bool validateResponse = false, CancellationToken cancel = default) where T : class
                => await SendHttpRequest<string, T>(requestType, apiMethod, string.Empty, 
                    (opt, headers) => { opt.ValidateErrorsInResponse = validateResponse; }, cancel);

        public async Task<TResp> SendHttpRequest<TReq, TResp>(HttpMethod requestType, string apiMethod, TReq body, 
            Action<HttpSenderOptions, CustomHttpHeaders> cfgAction, CancellationToken cancel = default)
                where TResp : class
        {
            var uri = new Uri(Client.BaseAddress, apiMethod);

            using (var request = new HttpRequestMessage(requestType, uri))
            {
                var senderOptions = new HttpSenderOptions();
                var headers = new CustomHttpHeaders();

                cfgAction?.Invoke(senderOptions, headers);

                request.Content = CreateContent(body, senderOptions.SerializingSettings);
                request.AppendHeaders(headers);

                HttpResponseMessage response;

                try
                {
                    _logger.LogDebug($"Request: [{requestType.ToString().ToUpper()}] {uri.AbsoluteUri}");

                    response = await Client.SendAsync(request, cancel);
                }
                catch (TaskCanceledException cex)
                {
                    throw new HttpExc($"Client cancel task: {cex.Message}");
                }
                catch (TimeoutException tex)
                {
                    throw new HttpExc($"Connection timeout: {tex.Message}");
                }
                catch (Exception ex)
                {
                    throw new HttpExc(ex.Message, ex.InnerException);
                }

                _logger.LogDebug("Response: " + response.Content);

                await CheckResponseStatusCode(response);

                var bodyAsStr = await HttpSenderHelper.ExtractBodyAsync(response.Content);

                if (senderOptions.ValidateErrorsInResponse && CheckResponseBody(bodyAsStr, out var errMsg))
                    throw new HttpExc(HttpStatusCode.InternalServerError, errMsg);

                return JsonConvert.DeserializeObject<TResp>(bodyAsStr, senderOptions.DeserializingSettings);
            }
        }
    }
}
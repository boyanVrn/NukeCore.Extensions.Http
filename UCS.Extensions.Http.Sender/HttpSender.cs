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
using UCS.Extensions.Http.Sender.Settings;

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

        private static HttpContent CreateContent<T>(T body, JsonSerializerSettings serializeSettings)
        {
            if (body == null) return null;

            switch (body)
            {
                case byte[] bytes:
                    {
                        var content = new ByteArrayContent(bytes);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        return content;
                    }
                case string str:
                    {
                        var content = new StringContent(str, Encoding.UTF8);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        return content;
                    }
                default:
                    {
                        var postedData = JsonConvert.SerializeObject(body, serializeSettings);

                        var content = new StringContent(postedData, Encoding.UTF8);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        return content;
                    }
            }
        }

        protected virtual async Task CheckResponseStatusCode(HttpResponseMessage src)
        {
            if (src.StatusCode != HttpStatusCode.OK)
                throw new HttpExc(src.StatusCode, src.ReasonPhrase + Environment.NewLine + await HttpSenderHelper.ExtractBodyAsync(src.Content));
        }

        protected virtual bool CheckResponseBody(string body, out string errText)
        {
            errText = string.Empty;
            try
            {
                var jToken = JToken.Parse(body);

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


        public async Task SendHttpRequest(HttpMethod requestType, string apiMethod, object postedData,
            bool validateResponse = false, CancellationToken cancel = default)
                => await SendHttpRequest<object, object>(requestType, apiMethod, postedData,
                    (opt, headers) => { opt.ValidateErrorsInResponse = validateResponse; }, cancel);

        public async Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, object postedData,
            bool validateResponse = false, CancellationToken cancel = default) where T : class
                => await SendHttpRequest<object, T>(requestType, apiMethod, postedData,
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

                try
                {
                    _logger.LogDebug($"Request: [{requestType.ToString().ToUpper()}] {uri.AbsoluteUri}");

                    var response = await Client.SendAsync(request, cancel);

                    await CheckResponseStatusCode(response);

                    var bodyAsStr = await HttpSenderHelper.ExtractBodyAsync(response.Content);

                    _logger.LogDebug("Response: " + bodyAsStr);

                    if (senderOptions.ValidateErrorsInResponse && CheckResponseBody(bodyAsStr, out var errMsg))
                        throw new HttpExc(HttpStatusCode.InternalServerError, errMsg);

                    return JsonConvert.DeserializeObject<TResp>(bodyAsStr, senderOptions.DeserializingSettings);
                }
                catch (TaskCanceledException cex)
                {
                    throw new HttpExc($"Client cancel task: {cex.Message}");
                }
                catch (TimeoutException tex)
                {
                    throw new HttpExc($"Connection timeout: {tex.Message}");
                }
                catch (HttpExc)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new HttpExc(ex.Message, ex.InnerException);
                }

            }
        }
    }
}
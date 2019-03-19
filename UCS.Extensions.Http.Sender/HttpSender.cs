using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
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

        protected virtual bool CheckResponse(string src, out string errText)
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
                return false;
            }
        }

        public async Task SendHttpRequest(HttpMethod requestType, string apiMethod, string postedData, 
            bool validateResponse = false, CancellationToken cancel = default)
                => await SendHttpRequest<string, object>(requestType, apiMethod, string.Empty, null, validateResponse, cancel);

        public async Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, string postedData,
            bool validateResponse = false, CancellationToken cancel = default) where T : class
                => await SendHttpRequest<string, T>(requestType, apiMethod, postedData, null, validateResponse, cancel);

        public async Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod,
            bool validateResponse = false, CancellationToken cancel = default) where T : class
                => await SendHttpRequest<string, T>(requestType, apiMethod, string.Empty, null, validateResponse, cancel);

        public async Task<TResp> SendHttpRequest<TReq, TResp>(HttpMethod requestType, string apiMethod, TReq body, Action<CustomHttpHeaders, JsonSerializerSettings> cfgAction,
            bool validateResponse = false, CancellationToken cancel = default)
                where TResp : class
        {
            using (var request = new HttpRequestMessage(requestType, apiMethod))
            {
                var serializeCfg = new JsonSerializerSettings();
                var headers = new CustomHttpHeaders();
                cfgAction?.Invoke(headers, serializeCfg);

                request.Content = CreateContent(body, serializeCfg);
                request.AppendHeaders(headers);

                HttpResponseMessage response;

                try
                {
                    _logger.LogDebug($"Request: [{requestType.ToString().ToUpper()}] {apiMethod}");

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

                await response.CheckStatusCode();

                var bodyAsStr = await HttpSenderHelper.ExtractBodyAsync(response.Content);

                if (validateResponse && CheckResponse(bodyAsStr, out var errMsg))
                    throw new ExternalException(errMsg);

                return JsonConvert.DeserializeObject<TResp>(bodyAsStr);
            }
        }
    }
}
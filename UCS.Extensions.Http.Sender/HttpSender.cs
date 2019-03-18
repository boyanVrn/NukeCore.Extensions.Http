using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCS.Extensions.Http.Errors;

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

        protected virtual bool CheckResponse(string src, out string errText)
        {
            errText = string.Empty;
            try
            {
                var jToken = JToken.Parse(src);
                if (jToken.First == null) return false;

                errText = jToken["error"]?.Value<string>();
                return !string.IsNullOrEmpty(errText);
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }
        public async Task SendHttpRequest(HttpMethod requestType, string apiMethod, string postedData, bool validateResponse = false, CancellationToken cancel = default)
            => await SendHttpRequest<object>(requestType, apiMethod, string.Empty, validateResponse, cancel);

        public async Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, bool validateResponse = false, CancellationToken cancel = default) where T : class
            => await SendHttpRequest<T>(requestType, apiMethod, string.Empty, validateResponse, cancel);

        public async Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, string postedData, bool validateResponse = false, CancellationToken cancel = default)
            where T : class
        {
            using (var request = new HttpRequestMessage(requestType, apiMethod))
            using (var httpContent = new StringContent(postedData, Encoding.UTF8, "application/json"))
            {
                request.Content = httpContent;
                HttpResponseMessage response;

                try
                {
                    _logger.LogDebug($"Request: [{requestType.ToString().ToUpper()}] {apiMethod}");

                    if (!string.IsNullOrEmpty(postedData))
                        _logger.LogDebug($"PostedData: {postedData}");

                    response = await Client.SendAsync(request, cancel);
                }
                catch (TaskCanceledException cex)
                {
                    throw new HttpExc(HttpErrorEnum.ERR_HTTP_CLIENT_CANCEL_TASK, cex.Message);
                }
                catch (Exception ex)
                {
                    throw new HttpExc(HttpErrorEnum.ERR_HTTP_CONNECTION_TIMEOUT, ex.Message);
                }

                _logger.LogDebug("Response: " + response.Content);

                response.ValidateResponseCode();

                var bodyAsStr = await HttpSenderHelper.ExtractBodyAsync(response.Content);

                if (validateResponse && CheckResponse(bodyAsStr, out var errMsg))
                    throw new ExternalException(errMsg);

                return JsonConvert.DeserializeObject<T>(bodyAsStr);
            }
        }
    }
}
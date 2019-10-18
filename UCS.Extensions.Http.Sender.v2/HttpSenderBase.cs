using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UCS.Extensions.Http.Common.Helpers;
using UCS.Extensions.Http.Common.Models;
using UCS.Extensions.Http.Errors;
using UCS.Extensions.Http.Errors.v2;
using UCS.Extensions.Http.Models.v2;

namespace UCS.Extensions.Http.Sender.v2
{

    /// <summary>
    /// http sander abstract base class
    /// </summary>
    public abstract class HttpSenderBase //: IHttpSender
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly HttpSenderOptions _options;

        /// <summary>
        /// constructor, create new instance of HttpSender
        /// </summary>
        /// <param name="client">System.Net.Http.HttpClient</param>
        /// <param name="options"></param>
        /// <param name="logger">Microsoft.Extensions.Logging.ILogger</param>
        protected HttpSenderBase(HttpClient client, HttpSenderOptions options, ILogger logger)
        {
            _client = client;
            _options = options;
            _logger = logger;
        }

        /// <summary>
        /// validate http code in responce, in base valid only 200
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        /// <exception cref="HttpExc"></exception>
        protected abstract bool CheckResponseStatusCode(HttpResponseMessage msg);

        /// <summary>
        /// validate responce bode, if contains 'error' field return true
        /// </summary>
        /// <param name="body">responce body</param>
        /// <param name="errMsg"></param>
        /// <returns>true or false</returns>
        protected abstract bool TryExtractErrorFromBody<T>(T body, out string errMsg);

        /// <summary>
        /// deserialize http response body to class
        /// <param name="str">string to deserializing</param>
        /// <param name="options">deserialize options</param>
        /// </summary>
        /// <returns>T resolver</returns>
        protected abstract IResponse<T> Deserialize<T>(string str, HttpSenderOptions options)
            where T : new();

        /// <summary>
        /// serialize http request class to string
        /// <param name="obj">object to serializing</param>
        /// <param name="options">serialize options</param>
        /// </summary>
        /// <returns>stringr</returns>
        protected abstract string Serialize<T>(T obj, HttpSenderOptions options);

        /// <summary>
        /// create request content by body type
        /// <param name="body">object query</param>
        /// <param name="options">options</param>
        /// </summary>
        /// <returns>stringr</returns>
        protected abstract HttpContent CreateContent<T>(T body, HttpSenderOptions options);

        /// <inheritdoc/>
        public async Task SendHttpRequest(HttpMethod requestType, string apiMethod, object postedData, CancellationToken cancel = default)
            => await SendHttpRequest<object, object>(requestType, apiMethod, postedData, cancel);

        /// <inheritdoc/>
        public async Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, object postedData, CancellationToken cancel = default) where T : new()
            => await SendHttpRequest<object, T>(requestType, apiMethod, postedData, cancel);

        /// <inheritdoc/>
        public async Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, CancellationToken cancel = default) where T : new()
            => await SendHttpRequest<string, T>(requestType, apiMethod, string.Empty, cancel);

        /// <inheritdoc/>
        public async Task<TResp> SendHttpRequest<TReq, TResp>(HttpMethod requestType, string apiMethod, TReq body, CancellationToken cancel = default,
            Action<HttpSenderOptions, CustomHttpHeaders> cfgAction = default)
            where TResp : new()
        {
            HttpSenderOptions options;
            var headers = new CustomHttpHeaders();

            if (cfgAction == default)
            {
                options = _options;
            }
            else
            {
                options = new HttpSenderOptions();
                cfgAction.Invoke(options, headers);
            }

            var content = DoCreateContent(body, options);
            return await SendHttpRequest<TResp>(requestType, apiMethod, content, cancel, options, headers);
        }

        /// <inheritdoc/>
        public async Task<IResponse<TResp>> SendHttpRequest<TResp>(HttpMethod requestType, string apiMethod, HttpContent content, 
            CancellationToken cancel = default, HttpSenderOptions options = default, CustomHttpHeaders headers = default)
            where TResp : new()
        {
            var uri = new Uri(_client.BaseAddress, apiMethod);
            var senderOptions = options ?? _options ?? new HttpSenderOptions();
            var senderHeaders = headers ?? new CustomHttpHeaders();

            using (var request = new HttpRequestMessage(requestType, uri))
            {
                request.Content = content;
                request.AppendHeaders(senderHeaders);

                try
                {
                    _logger.LogDebug($"Request: [{requestType.ToString().ToUpper()}] {uri.AbsoluteUri}");

                    var response = await _client.SendAsync(request, cancel);

                    var bodyAsStr = await HttpSenderHelper.ExtractBodyAsync(response.Content);

                    if (!CheckResponseStatusCode(response))
                        return ResponseFactory<TResp>.CreateInstance(new HttpException(response.ReasonPhrase + Environment.NewLine + bodyAsStr));

                    _logger.LogDebug("Response: " + bodyAsStr);
                    
                    return DoSerialize<TResp>(bodyAsStr, senderOptions);
                }
                catch (TaskCanceledException cex)
                {
                    return ResponseFactory<TResp>.CreateInstance(new HttpException($"Client cancel task: {cex.Message}"));
                }
                catch (TimeoutException tex)
                {
                    return ResponseFactory<TResp>.CreateInstance(new HttpException($"Connection timeout: {tex.Message}"));
                }
                catch (Exception ex)
                {
                    return ResponseFactory<TResp>.CreateInstance(new HttpException(ex.Message, ex.InnerException));
                }
            }
        }

        private HttpContent DoCreateContent<T>(T body, HttpSenderOptions options)
        {
            return body == null ? null : CreateContent(body, options);
        }

        private IResponse<T> DoSerialize<T>(string str, HttpSenderOptions options)
            where T : new()
        {
            if (string.IsNullOrEmpty(str)) return ResponseFactory<T>.CreateInstance(new T()) ;
            if (typeof(T).IsValueType || typeof(T) == typeof(string)) return ResponseFactory<T>.CreateInstance((T)Convert.ChangeType(str, typeof(T)));

            return Deserialize<T>(str, options);
        }
    }
}
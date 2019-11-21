using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NukeCore.Extensions.Http.Common.Helpers;
using NukeCore.Extensions.Http.Common.Models;
using NukeCore.Extensions.Http.Errors;
using NukeCore.Extensions.Http.Models;
using NukeCore.Extensions.Http.Models.Base.Resolvers;
using NukeCore.Extensions.Http.Models.Factory;

namespace NukeCore.Extensions.Http.Sender
{

    //TODO fill xml doc 
    /// <summary>
    /// http sander abstract base class
    /// </summary>
    public abstract class HttpSenderBase : IHttpSender
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly HttpSenderOptions _options;
        /// <summary>
        /// 
        /// </summary>
        protected readonly IResponseFactory ResponseFactory;


        /// <summary>
        /// constructor, create new instance of HttpSender
        /// </summary>
        /// <param name="client">System.Net.Http.HttpClient</param>
        /// <param name="options"></param>
        /// <param name="logger">Microsoft.Extensions.Logging.ILogger</param>
        /// <param name="responseFactory"></param>
        /// 
        protected HttpSenderBase(HttpClient client, HttpSenderOptions options, IResponseFactory responseFactory, ILogger logger)
        {
            _client = client;
            _options = options;
            _logger = logger;
            ResponseFactory = responseFactory;
        }

        /// <summary>
        /// validate http code in responce, in base valid only 200
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual bool HasNotOkStatusCode(HttpResponseMessage msg)
        {
            return msg.StatusCode != HttpStatusCode.OK;
        }

        /// <summary>
        /// validate responce bode, if contains 'error' field return true
        /// </summary>
        /// <param name="body">responce body</param>
        /// <param name="err"></param>
        /// <returns>true or false</returns>
        protected abstract bool TryExtractErrorFromBody<T>(T body, out FailBase err);
        // where TOut : IFail, new();

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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="apiMethod"></param>
        /// <param name="postedData"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public async Task SendHttpRequest(HttpMethod requestType, string apiMethod, object postedData, CancellationToken cancel = default)
            => await SendHttpRequest<object, object>(requestType, apiMethod, postedData, cancel);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="apiMethod"></param>
        /// <param name="postedData"></param>
        /// <param name="cancel"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IResponse<T>> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, object postedData, CancellationToken cancel = default) where T : new()
            => await SendHttpRequest<object, T>(requestType, apiMethod, postedData, cancel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="apiMethod"></param>
        /// <param name="cancel"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<IResponse<T>> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, CancellationToken cancel = default) where T : new()
            => await SendHttpRequest<string, T>(requestType, apiMethod, string.Empty, cancel);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="apiMethod"></param>
        /// <param name="body"></param>
        /// <param name="cancel"></param>
        /// <param name="cfgAction"></param>
        /// <typeparam name="TReq"></typeparam>
        /// <typeparam name="TResp"></typeparam>
        /// <returns></returns>
        public async Task<IResponse<TResp>> SendHttpRequest<TReq, TResp>(HttpMethod requestType, string apiMethod, TReq body, CancellationToken cancel = default,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="apiMethod"></param>
        /// <param name="content"></param>
        /// <param name="cancel"></param>
        /// <param name="options"></param>
        /// <param name="headers"></param>
        /// <typeparam name="TResp"></typeparam>
        /// <returns></returns>
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

                using (var cts = CreateCancellationTokenSource(senderOptions.RequestTimeout, cancel))
                {

                    try
                    {
                        _logger.LogDebug($"Request: [{requestType.ToString().ToUpper()}] {uri.AbsoluteUri}");
                        _logger.LogDebug(await content.ReadAsStringAsync());
                        var response = await _client.SendAsync(request, cts?.Token ?? cancel);

                        var bodyAsStr = await HttpSenderHelper.ExtractBodyAsync(response.Content);

                        _logger.LogDebug("Response: " + bodyAsStr);

                        return HasNotOkStatusCode(response)
                            ? ResponseFactory.CreateFault<TResp>(new HttpFail(response.StatusCode, response.ReasonPhrase))
                            : DoDeserialize<TResp>(bodyAsStr, senderOptions);
                    }
                    catch (OperationCanceledException oex)
                    {
                        var errMsg = cancel.IsCancellationRequested
                            ? $"Client cancel task: {oex.Message}"
                            : $"Connection timeout: {oex.Message}";

                        return ResponseFactory.CreateFault<TResp>(new HttpFail(errMsg));
                    }
                    catch (Exception ex)
                    {
                        return ResponseFactory.CreateFault<TResp>(new HttpFail(ex.Message, ex.InnerException));
                    }
                }
            }
        }

        //TODO перенести в хелпер
        private static CancellationTokenSource CreateCancellationTokenSource(TimeSpan timeout, CancellationToken cancel)
        {
            if (timeout == Timeout.InfiniteTimeSpan) return null;

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancel);
            cts.CancelAfter(timeout);

            return cts;
        }

        private HttpContent DoCreateContent<T>(T body, HttpSenderOptions options)
        {
            return body == null ? null : CreateContent(body, options);
        }

        private IResponse<T> DoDeserialize<T>(string str, HttpSenderOptions options)
            where T : new()
        {
            if (string.IsNullOrEmpty(str)) return ResponseFactory.CreateSuccess(new T());
            if (typeof(T).IsValueType || typeof(T) == typeof(string)) return ResponseFactory.CreateSuccess((T)Convert.ChangeType(str, typeof(T)));

            return Deserialize<T>(str, options);
        }
    }
}
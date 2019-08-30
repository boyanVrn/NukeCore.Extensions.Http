using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UCS.Extensions.Http.Common.Models;

namespace UCS.Extensions.Http.Sender.v2
{
    /// <summary>
    /// http sender methods interface
    /// </summary>
    public interface IHttpSender
    {
        /// <summary>
        /// use in case when request body exists, but response body no needed
        /// </summary>
        /// <param name="requestType">request type</param>
        /// <param name="apiMethod">url path</param>
        /// <param name="postedData">request body</param>
        /// <param name="validateResponse">if set then check response by HasErrorInResponseBody method</param>
        /// <param name="cancel">cancel query token</param>
        /// <returns>void or exception</returns>
        Task SendHttpRequest(HttpMethod requestType, string apiMethod, object postedData,
            bool validateResponse = true, CancellationToken cancel = default);

        /// <summary>
        /// use in case when request body is not exists, but response body is needed
        /// </summary>
        /// <param name="requestType">request type</param>
        /// <param name="apiMethod">url path</param>
        /// <param name="validateResponse">if set then check response by HasErrorInResponseBody method</param>
        /// <param name="cancel">cancel query token</param>
        /// <typeparam name="T">responce deserialize type</typeparam>
        /// <returns>class or exception</returns>
        Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod,
            bool validateResponse = true, CancellationToken cancel = default) where T : new();

        /// <summary>
        /// use in case when request body exists, and response body needed too
        /// </summary>
        /// <param name="requestType">request type</param>
        /// <param name="apiMethod">url path</param>
        /// <param name="postedData">request body</param>
        /// <param name="validateResponse">if set then check response by HasErrorInResponseBody method</param>
        /// <param name="cancel">cancel query token</param>
        /// <typeparam name="T">responce deserialize type</typeparam>
        /// <returns>class or exception</returns>
        Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, object postedData,
            bool validateResponse = true, CancellationToken cancel = default) where T : new();

        /// <summary>
        /// base extended http send method. use then need flex custom tuning
        /// </summary>
        /// <param name="requestType">request type</param>
        /// <param name="apiMethod"></param>
        /// <param name="body">url path</param>
        /// <param name="cfgAction">params configuring action</param>
        /// <param name="cancel">cancel query token</param>
        /// <typeparam name="TReq">request serialization type</typeparam>
        /// <typeparam name="TResp">responce deserialize type</typeparam>
        /// <returns>class or exception</returns>
        Task<TResp> SendHttpRequest<TReq, TResp>(HttpMethod requestType, string apiMethod, TReq body,
            Action<HttpSenderOptions, CustomHttpHeaders> cfgAction, CancellationToken cancel = default)
                where TResp : new();

        /// <summary>
        /// base extended http send method. use then need flex custom tuning
        /// </summary>
        /// <param name="requestType">request type</param>
        /// <param name="apiMethod"></param>
        /// <param name="headers">http request custom headers</param>
        /// <param name="options">http sender custom options</param>
        /// <param name="cancel">cancel query token</param>
        /// <param name="content">http request content</param>
        /// <typeparam name="TResp">responce deserialize type</typeparam>
        /// <returns>class or exception</returns>
        Task<TResp> SendHttpRequest<TResp>(HttpMethod requestType, string apiMethod, HttpContent content,
            CustomHttpHeaders headers = default, HttpSenderOptions options = default, CancellationToken cancel = default)
                where TResp : new();
    }
}
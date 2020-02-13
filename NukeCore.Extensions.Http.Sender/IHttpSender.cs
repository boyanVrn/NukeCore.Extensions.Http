using NukeCore.Extensions.Monad.Response.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NukeCore.Extensions.Http.Common.Settings;

namespace NukeCore.Extensions.Http.Sender
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
        /// <param name="cancel">cancel query token</param>
        /// <returns>void or exception</returns>
        Task SendHttpRequest(HttpMethod requestType, string apiMethod, object postedData,
            CancellationToken cancel = default);

        /// <summary>
        /// use in case when request body is not exists, but response body is needed
        /// </summary>
        /// <param name="requestType">request type</param>
        /// <param name="apiMethod">url path</param>
        /// <param name="cancel">cancel query token</param>
        /// <typeparam name="T">responce deserialize type</typeparam>
        /// <returns>class or exception</returns>
        Task<IResponse<T>> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, CancellationToken cancel = default) 
            where T : new();

        /// <summary>
        /// use in case when request body exists, and response body needed too
        /// </summary>
        /// <param name="requestType">request type</param>
        /// <param name="apiMethod">url path</param>
        /// <param name="postedData">request body</param>
        /// <param name="cancel">cancel query token</param>
        /// <typeparam name="T">responce deserialize type</typeparam>
        /// <returns>class or exception</returns>
        Task<IResponse<T>> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, object postedData, CancellationToken cancel = default) 
            where T : new();

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
        Task<IResponse<TResp>> SendHttpRequest<TReq, TResp>(HttpMethod requestType, string apiMethod, TReq body, 
            CancellationToken cancel = default, Action<HttpSenderOptions, CustomHttpHeaders> cfgAction = default)
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
        Task<IResponse<TResp>> SendHttpRequest<TResp>(HttpMethod requestType, string apiMethod, HttpContent content,
            CancellationToken cancel = default, HttpSenderOptions options = default, CustomHttpHeaders headers = default)
                where TResp : new();
    }
}
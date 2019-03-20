using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UCS.Extensions.Http.Sender.Entities;

namespace UCS.Extensions.Http.Sender
{
    public interface IHttpSender
    {
        Task SendHttpRequest(HttpMethod requestType, string apiMethod, string postedData,
            bool validateResponse = false, CancellationToken cancel = default);

        Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod,
            bool validateResponse = false, CancellationToken cancel = default) where T : class;

        Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, string postedData,
            bool validateResponse = false, CancellationToken cancel = default) where T : class;

        Task<TResp> SendHttpRequest<TReq, TResp>(HttpMethod requestType, string apiMethod, TReq body,
            Action<HttpSenderOptions, CustomHttpHeaders> cfgAction, CancellationToken cancel = default)
            where TResp : class;
    }
}
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UCS.Extensions.Http.Sender
{
    public interface IHttpSender
    {
        Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod) where T : class;
        Task<T> SendHttpRequest<T>(HttpMethod requestType, string apiMethod, string postedData,
            bool validateResponse = false, CancellationToken cancel = default) where T : class;
    }
}

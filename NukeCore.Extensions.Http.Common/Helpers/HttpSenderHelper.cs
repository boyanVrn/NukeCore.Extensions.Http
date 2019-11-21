using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NukeCore.Extensions.Http.Common.Models;

namespace NukeCore.Extensions.Http.Common.Helpers
{
    public static class HttpSenderHelper
    {
        //private const string CNT_CLIENT_ID = "x-http-client-uuid";

        public static void AppendHeaders(this HttpRequestMessage src, CustomHttpHeaders headersEx)
        {
            foreach (var h in headersEx)
            {
                if (string.IsNullOrEmpty(h.Key) || h.Value == null || !h.Value.Any()) continue;
                //if (h.Key == CNT_CLIENT_ID) continue;
                src.Headers.Add(h.Key, h.Value);
            }
        }

        public static async Task<string> ExtractBodyAsync(HttpContent content) => content == null
            ? string.Empty
            : Encoding.UTF8.GetString(await content.ReadAsByteArrayAsync());

        //public static void GetHttpClientId(this HttpRequestMessage src, CustomHttpHeaders headersEx)
        //{
        //    foreach (var h in headersEx)
        //    {
        //        if (string.IsNullOrEmpty(h.Key) || h.Value == null || !h.Value.Any()) continue;
        //        if (h.Key == CNT_CLIENT_ID) continue;
        //        src.Headers.Add(h.Key, h.Value);
        //    }
        //}

    }
}

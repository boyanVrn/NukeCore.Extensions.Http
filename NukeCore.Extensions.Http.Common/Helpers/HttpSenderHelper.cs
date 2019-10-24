using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UCS.Extensions.Http.Common.Models;

namespace UCS.Extensions.Http.Common.Helpers
{
    public static class HttpSenderHelper
    {
        public static void AppendHeaders(this HttpRequestMessage src, CustomHttpHeaders headersEx)
        {
            foreach (var h in headersEx)
            {
                if (string.IsNullOrEmpty(h.Key) || h.Value == null || !h.Value.Any()) continue;
                src.Headers.Add(h.Key, h.Value);
            }
        }
        public static async Task<string> ExtractBodyAsync(HttpContent content) => content == null
            ? string.Empty
            : Encoding.UTF8.GetString(await content.ReadAsByteArrayAsync());

    }
}

using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UCS.Extensions.Http.Sender.Entities;

namespace UCS.Extensions.Http.Sender
{
    public static class HttpSenderHelper
    {
        public static async Task<string> ExtractBodyAsync(HttpContent content) => content == null ? string.Empty : Encoding.UTF8.GetString(await content.ReadAsByteArrayAsync());

        public static void AppendHeaders(this HttpRequestMessage src, CustomHttpHeaders headersEx)
        {
            foreach (var h in headersEx)
            {
                if (string.IsNullOrEmpty(h.Key) || h.Value == null || !h.Value.Any()) continue;
                src.Headers.Add(h.Key, h.Value);
            }
        }
    }
}

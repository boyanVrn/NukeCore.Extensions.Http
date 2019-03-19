using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UCS.Extensions.Http.Errors;
using UCS.Extensions.Http.Sender.Entities;

namespace UCS.Extensions.Http.Sender
{
    internal static class HttpSenderHelper
    {
        public static async Task CheckStatusCode(this HttpResponseMessage src)
        {
            var errMsg = $"Http exception. StatusCode: {(int)src.StatusCode}";

            if (src.StatusCode != HttpStatusCode.OK)
                throw new HttpExc(src.StatusCode, await ExtractBodyAsync(src.Content), errMsg);
        }

        public static async Task<string> ExtractBodyAsync(HttpContent content) => content == null ? string.Empty : Encoding.UTF8.GetString(await content.ReadAsByteArrayAsync());

        public static void AppendHeaders(this HttpRequestMessage src, CustomHttpHeaders headersEx)
        {
            foreach (var h in headersEx)
            {
                if (string.IsNullOrEmpty(h.Key) || string.IsNullOrEmpty(h.Value)) continue;
                src.Headers.Add(h.Key, h.Value);
            }
        }
    }
}

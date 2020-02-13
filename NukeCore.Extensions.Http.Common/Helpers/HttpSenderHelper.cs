using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NukeCore.Extensions.Http.Common.Settings;

namespace NukeCore.Extensions.Http.Common.Helpers
{
    public static class HttpSenderHelper
    {
        private const string MtNameAppJson = "application/json";
        private const string MtNameAppXml = "application/xml";
        private const string MtNameBytes = "application/octet-stream";

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

        public static ByteArrayContent CreateByteArrayContent(byte[] bytes)
        {
            var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue(MtNameBytes);
            return content;
        }

        public static StringContent CreateStringContent(string str, bool isXml = false)
        {
            var content = new StringContent(str, Encoding.UTF8);
            content.Headers.ContentType = new MediaTypeHeaderValue(isXml ? MtNameAppXml : MtNameAppJson);
            return content;
        }

        public static CancellationTokenSource CreateCancellationTokenSource(TimeSpan timeout, CancellationToken cancel)
        {
            if (timeout == Timeout.InfiniteTimeSpan) return null;

            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancel);
            cts.CancelAfter(timeout);

            return cts;
        }
    }
}

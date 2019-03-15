using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UCS.Extensions.Http.Errors;

namespace UCS.Extensions.Http.Sender
{
    public static class HttpSenderHelper
    {
        public static void ValidateResponseCode(this HttpResponseMessage src)
        {
            var errMsg = $"Http exception. StatusCode: {(int)src.StatusCode}";

            switch (src.StatusCode)
            {
                //401
                case HttpStatusCode.Unauthorized:
                    throw new HttpExc(HttpErrorEnum.ERR_HTTP_AUTH, $"{errMsg} Unauthorized");
                //403
                case HttpStatusCode.Forbidden:
                    throw new HttpExc(HttpErrorEnum.ERR_HTTP_ACCESS_DENIED, $"{errMsg} Forbidden");
                //404
                case HttpStatusCode.NotFound:
                    throw new HttpExc(HttpErrorEnum.ERR_HTTP_NOT_FOUND, $"{errMsg} Not Found");
                //408 504
                case var state when state == HttpStatusCode.RequestTimeout || state == HttpStatusCode.GatewayTimeout:
                    throw new HttpExc(HttpErrorEnum.ERR_HTTP_CONNECTION_TIMEOUT, $"{errMsg} Connection Timeout");
                //500
                case HttpStatusCode.InternalServerError:
                    throw new HttpExc(HttpErrorEnum.ERR_HTTP_SERVER_INTERNAL, $"{errMsg} Internal Server Error");
            }
        }

        public static async Task<string> ExtractBodyAsync(HttpContent content) => content == null ? string.Empty : Encoding.UTF8.GetString(await content.ReadAsByteArrayAsync());
    }
}

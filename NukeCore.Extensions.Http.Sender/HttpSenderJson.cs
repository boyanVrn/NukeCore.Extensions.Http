using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UCS.Extensions.Http.Common.Models;
using UCS.Extensions.Http.Errors.v2;
using UCS.Extensions.Http.Models.v2;

namespace UCS.Extensions.Http.Sender.v2
{
    /// <summary>
    /// http json sender class
    /// </summary>
    public class HttpSenderJson : HttpSenderBase
    {
        /// <summary>
        /// constructor, create new instance of HttpSender
        /// </summary>
        /// <param name="client">System.Net.Http.HttpClient</param>
        /// <param name="options">http sender ext params</param>
        /// <param name="logger">Microsoft.Extensions.Logging.ILogger</param>
        public HttpSenderJson(HttpClient client, HttpSenderOptions options, ILogger logger) : base(client, options, logger) { }


        /// <inheritdoc/>
        protected override bool TryExtractErrorFromBody<T>(T body, out string msg)
        {
            msg = string.Empty;

            if (!(body is JToken jBody)) return false;

            if (!jBody.HasValues || jBody.First == null) return false;
            var errMsg = jBody["error"]?.Value<string>();

            if (string.IsNullOrEmpty(errMsg)) return false;

            msg = errMsg;
            return true;
        }

        /// <inheritdoc/>
        protected override IResponse<T> Deserialize<T>(string str, HttpSenderOptions options)
        {
            var jBody = JToken.Parse(str);

            if (options.ValidateErrorsInResponse && TryExtractErrorFromBody(jBody, out var errMsg))
                return ResponseBase<T>.CreateFault(new HttpFail(errMsg));

            var result = jBody.ToObject<T>(JsonSerializer.Create(options.JsonParseSettings.Deserializing));
            return ResponseBase<T>.CreateSuccess(result);
        }

        /// <inheritdoc/>
        protected override string Serialize<T>(T obj, HttpSenderOptions options)
        {
            return JsonConvert.SerializeObject(obj, options.JsonParseSettings.Serializing);
        }

        /// <inheritdoc/>
        protected override HttpContent CreateContent<T>(T body, HttpSenderOptions options)
        {
            switch (body)
            {
                case byte[] bytes:
                    {
                        var content = new ByteArrayContent(bytes);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        return content;
                    }

                case string str:
                    {
                        var content = new StringContent(str, Encoding.UTF8);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        return content;
                    }

                default:
                    {
                        var content = new StringContent(Serialize(body, options), Encoding.UTF8);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        return content;
                    }
            }
        }
    }
}
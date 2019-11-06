using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NukeCore.Extensions.Http.Common.Models;
using NukeCore.Extensions.Http.Models;
using NukeCore.Extensions.Http.Models.Base.Resolvers;

namespace NukeCore.Extensions.Http.Sender
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
        protected override bool TryExtractErrorFromBody<T>(T body, out FailBase err)
        {
            err = null;

            if (!(body is JToken jBody)) return false;

            if (!jBody.HasValues || jBody.First == null) return false;
            var msg = jBody["error"]?.Value<string>();
            var code = jBody["code"]?.Value<string>();

            if (string.IsNullOrEmpty(msg)) return false;

            err = FailBase.CreateInstance(code, msg);
            return true;
        }

        /// <inheritdoc/>
        protected override IResponse<T> Deserialize<T>(string str, HttpSenderOptions options)
        {
            var jBody = JToken.Parse(str);

            if (options.ValidateErrorsInResponse && TryExtractErrorFromBody(jBody, out var err))
                return ResponseBase<T>.CreateFault(err);

            var result = jBody.ToObject<T>(JsonSerializer.Create(options.JsonParseSettings.Deserializing));
            return ResponseBase<T>.CreateSuccess(result);
        }

        /// <inheritdoc/>
        protected override string Serialize<T>(T obj, HttpSenderOptions options)
        {
            if (obj == null) return string.Empty;
            if (obj is string s) return s;

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
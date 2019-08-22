using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UCS.Extensions.Http.Common.Helpers;
using UCS.Extensions.Http.Common.Models;
using UCS.Extensions.Http.Errors;

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
        protected override async Task CheckResponseStatusCode(HttpResponseMessage src)
        {
            if (src.StatusCode != HttpStatusCode.OK)
                throw new HttpExc(src.StatusCode, src.ReasonPhrase + Environment.NewLine + await HttpSenderHelper.ExtractBodyAsync(src.Content));
        }

        /// <inheritdoc/>
        protected override bool HasErrorInResponseBody(string body, out string errText)
        {
            errText = string.Empty;

            if (string.IsNullOrEmpty(body))
            {
                errText = "empty json";
                return false;
            }

            try
            {
                var jToken = JToken.Parse(body);

                if (!jToken.HasValues || jToken.First == null) return false;

                errText = jToken["error"]?.Value<string>();
                return !string.IsNullOrEmpty(errText);
            }
            catch (JsonReaderException)
            {
                errText = "Json parse error";
                return false;
            }
        }

        /// <inheritdoc/>
        protected override T Deserialize<T>(string str, HttpSenderOptions options)
        {
            return JsonConvert.DeserializeObject<T>(str, options.JsonParseSettings.Deserializing);
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
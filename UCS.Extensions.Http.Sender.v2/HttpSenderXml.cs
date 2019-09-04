using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using UCS.Extensions.Http.Common.Helpers;
using UCS.Extensions.Http.Common.Models;
using UCS.Extensions.Http.Errors;
using UCS.Extensions.Http.Sender.v2.Additional;

namespace UCS.Extensions.Http.Sender.v2
{
    /// <summary>
    /// http xml sender class
    /// </summary>
    public class HttpSenderXml : HttpSenderBase
    {
        /// <summary>
        /// constructor, create new instance of HttpSender
        /// </summary>
        /// <param name="client">System.Net.Http.HttpClient</param>
        /// <param name="options">http sender ext params</param>
        /// <param name="logger">Microsoft.Extensions.Logging.ILogger</param>
        public HttpSenderXml(HttpClient client, HttpSenderOptions options, ILogger logger) : base(client, options, logger) { }

        /// <inheritdoc/>
        protected override async Task CheckResponseStatusCode(HttpResponseMessage src)
        {
            if (src.StatusCode != HttpStatusCode.OK)
                throw new HttpExc(src.StatusCode, src.ReasonPhrase + Environment.NewLine + await HttpSenderHelper.ExtractBodyAsync(src.Content));
        }

        /// <inheritdoc/>
        protected override void CheckResponseBodyForError<T>(T body)
        {
            if (!(body is XDocument xBody)) return;

            var errCode = xBody?.XPathSelectElement("Exception/ErrorCode")?.Value ?? string.Empty;
            var errMsg = xBody?.XPathSelectElement("Exception/ErrorMessage")?.Value ?? string.Empty;

            if (!string.IsNullOrEmpty(errCode + errMsg))
                throw new HttpExc(HttpStatusCode.InternalServerError, $"{errCode}:{errMsg}");
        }

        /// <inheritdoc/>
        protected override T Deserialize<T>(string str, HttpSenderOptions options)
        {
            var doc = XDocument.Parse(str);

            if (options.ValidateErrorsInResponse) CheckResponseBodyForError(doc);
            if (options.XmlParseSettings.RemoveEmptyElements) doc.RemoveEmptyElements();

            return XmlUtils.CastXDocumentToObj<T>(doc);
        }

        /// <inheritdoc/>
        protected override string Serialize<T>(T obj, HttpSenderOptions options)
        {
            if (obj == null) return string.Empty;
            if (obj is string) return obj as string;

            return XmlSerializator.XmlSerializator.Serialize(obj);
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
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                        return content;
                    }

                default:
                    {
                        var content = new StringContent(Serialize(body, options), Encoding.UTF8);
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                        return content;
                    }
            }
        }
    }
}
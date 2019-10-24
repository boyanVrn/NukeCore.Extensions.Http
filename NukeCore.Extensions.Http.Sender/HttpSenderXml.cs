using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using UCS.Extensions.Http.Common.Additional;
using UCS.Extensions.Http.Common.Models;
using UCS.Extensions.Http.Errors.v2;
using UCS.Extensions.Http.Models.v2;

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
        protected override bool TryExtractErrorFromBody<T>(T body, out string msg)
        {
            msg = string.Empty;

            if (!(body is XDocument xBody)) return false;

            var errCode = xBody.XPathSelectElement("Exception/ErrorCode")?.Value ?? string.Empty;
            var errMsg = xBody.XPathSelectElement("Exception/ErrorMessage")?.Value ?? string.Empty;

            if (string.IsNullOrEmpty(errCode + errMsg))
                return false;

            msg = $"{errCode}:{errMsg}";
            return true;
        }

        /// <inheritdoc/>
        protected override IResponse<T> Deserialize<T>(string str, HttpSenderOptions options)
        {
            var doc = XDocument.Parse(str);

            if (options.ValidateErrorsInResponse && TryExtractErrorFromBody(doc, out var errMsg))
                return ResponseBase<T>.CreateFault(new HttpFail(errMsg));

            if (options.XmlParseSettings.RemoveEmptyElements) XmlUtils.RemoveEmptyElementsFrom(doc);

            return ResponseBase<T>.CreateSuccess(XmlUtils.CastXDocumentToObj<T>(doc));
        }

        /// <inheritdoc/>
        protected override string Serialize<T>(T obj, HttpSenderOptions options)
        {
            if (obj == null) return string.Empty;
            if (obj is string s) return s;

            if (options.XmlParseSettings.RemoveEmptyElements)
            {
                var doc = XmlUtils.CastObjToXDocument(obj);
                XmlUtils.RemoveEmptyElementsFrom(doc);

                return doc.ToString();
            }

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
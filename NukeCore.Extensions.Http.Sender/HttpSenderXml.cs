using System.Net.Http;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Extensions.Logging;
using NukeCore.Extensions.Http.Common.Helpers;
using NukeCore.Extensions.Http.Common.Settings;
using NukeCore.Extensions.Monad.Response.Factory;
using NukeCore.Extensions.Monad.Response.Models;
using NukeCore.Extensions.Monad.Response.Models.Base.Resolvers;
using NukeCore.Extensions.Shared.Utils;


namespace NukeCore.Extensions.Http.Sender
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
        /// <param name="responseFactory"></param>
        /// <param name="logger">Microsoft.Extensions.Logging.ILogger</param>
        protected HttpSenderXml(HttpClient client, HttpSenderOptions options, IResponseFactory responseFactory, ILogger<HttpSenderXml> logger)
            : base(client, options, responseFactory, logger) { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <param name="err"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected override bool TryExtractErrorFromBody<T>(T body, out FailBase err)
        {
            err = null;

            if (!(body is XDocument xBody)) return false;

            bool ExtractFromElement(out string errCode, out string errMsg)
            {
                errCode = xBody.XPathSelectElement("Exception/ErrorCode")?.Value ?? string.Empty;
                errMsg = xBody.XPathSelectElement("Exception/ErrorMessage")?.Value ?? string.Empty;

                return !string.IsNullOrEmpty(errCode + errMsg);
            }

            bool ExtractFromAttribute(out string errCode, out string errMsg)
            {
                var xData = xBody.Element("Data");

                errCode = xData?.Attribute("ErrorCode")?.Value ?? string.Empty;
                errMsg = xData?.Attribute("ErrorText")?.Value ?? string.Empty;

                return !string.IsNullOrEmpty(errCode + errMsg);
            }

            if (ExtractFromElement(out var sCode, out var sMsg) || ExtractFromAttribute(out sCode, out sMsg))
            {
                err = FailBase.CreateInstance(sCode, sMsg);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        protected override IResponse<T> Deserialize<T>(string str, HttpSenderOptions options)
        {
            var doc = XDocument.Parse(str);

            if (options.ValidateErrorsInResponse && TryExtractErrorFromBody(doc, out var err))
                return ResponseFactory.CreateFault<T>(err);

            if (options.XmlParseSettings.Deserialize.RemoveEmptyElements) XmlUtils.RemoveEmptyElementsFrom(doc);
            if (options.XmlParseSettings.Deserialize.RemoveNilElements) doc.RemoveNilElements();

            return ResponseFactory.CreateSuccess(XmlUtils.CastXDocumentToObj<T>(doc));
        }

        /// <inheritdoc/>
        protected override string Serialize<T>(T obj, HttpSenderOptions options)
        {
            if (obj == null) return string.Empty;
            if (obj is string s) return s;

            var doc = XmlUtils.CastObjToXDocument(obj);

            if (options.XmlParseSettings.Serialize.RemoveEmptyElements) XmlUtils.RemoveEmptyElementsFrom(doc);
            if (options.XmlParseSettings.Serialize.RemoveNilElements) doc.RemoveNilElements();

            return doc.ToString();
        }

        /// <inheritdoc/>
        protected override HttpContent CreateContent<T>(T body, HttpSenderOptions options)
        {
            if (body is byte[] bytes)
                return HttpSenderHelper.CreateByteArrayContent(bytes);

            return HttpSenderHelper.CreateStringContent(Serialize(body, options), true);
        }
    }
}
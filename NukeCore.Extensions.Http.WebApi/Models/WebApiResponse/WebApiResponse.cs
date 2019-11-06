using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NukeCore.Extensions.Http.WebApi.Helpers;

namespace NukeCore.Extensions.Http.WebApi.Models.WebApiResponse
{
    public abstract class WebApiResponse : IWebApiResponse
    {
        public abstract string AsString();
        public abstract string AsString(JsonSerializerSettings ss);
        public abstract OkObjectResult AsObject();

        public FileContentResult AsFile()
        {
            return new FileContentResult(AsString().ToUTF8ByteArray(), "application/octet-stream");
        }
    }
}

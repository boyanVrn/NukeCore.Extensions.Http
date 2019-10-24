using Microsoft.AspNetCore.Mvc;
using NukeCore.Extensions.Http.WebApi.Helpers;

namespace NukeCore.Extensions.Http.WebApi.Models.WebApiResponse
{
    public abstract class WebApiResponse : IWebApiResponse
    {
        public abstract string AsString();
        public abstract OkObjectResult AsObject();

        public FileContentResult AsFile()
        {
            return new FileContentResult(AsString().ToUTF8ByteArray(), "application/octet-stream");
        }
    }
}

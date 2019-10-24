using Microsoft.AspNetCore.Mvc;

namespace NukeCore.Extensions.Http.WebApi.Models.WebApiResponse
{
    public interface IWebApiResponse
    {
        string AsString();
        OkObjectResult AsObject();
        FileContentResult AsFile();
    }
}

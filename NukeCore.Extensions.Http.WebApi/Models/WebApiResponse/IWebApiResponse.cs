using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace NukeCore.Extensions.Http.WebApi.Models.WebApiResponse
{
    public interface IWebApiResponse
    {
        string AsString();
        string AsString(JsonSerializerSettings ss);
        OkObjectResult AsObject();
        FileContentResult AsFile();
    }
}

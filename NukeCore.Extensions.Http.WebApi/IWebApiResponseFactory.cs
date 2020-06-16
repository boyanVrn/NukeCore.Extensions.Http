using NukeCore.Extensions.Http.WebApi.Models.WebApiResponse;
using NukeCore.Extensions.Monad.Response.Models;

namespace NukeCore.Extensions.Http.WebApi
{
    public interface IWebApiResponseFactory
    {
        IWebApiResponse Ok();
        IWebApiResponse Ok<T>(T data);
        IWebApiResponse Error<T>(T error);
        IWebApiResponse CreateFrom<T>(IResponse<T> response);
    }
}

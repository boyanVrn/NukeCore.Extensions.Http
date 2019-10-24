using NukeCore.Extensions.Http.WebApi.Models.WebApiResponse;

namespace NukeCore.Extensions.Http.WebApi
{
    public interface IWebApiResponseFactory
    {
        IWebApiResponse Ok<T>(T data);
        IWebApiResponse Error<T>(T error);
    }
}

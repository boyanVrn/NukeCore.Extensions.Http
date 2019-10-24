using UCS.Extensions.Http.WebApi.Models.WebApiResponse;

namespace UCS.Extensions.Http.WebApi
{
    public interface IWebApiResponseFactory
    {
        IWebApiResponse Ok<T>(T data);
        IWebApiResponse Error<T>(T error);
    }
}

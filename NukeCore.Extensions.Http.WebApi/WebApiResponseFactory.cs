using UCS.Extensions.Http.Models.Base.Resolvers;
using UCS.Extensions.Http.WebApi.Models;
using UCS.Extensions.Http.WebApi.Models.WebApiResponse;

namespace UCS.Extensions.Http.WebApi
{

    public class WebApiResponseFactory : IWebApiResponseFactory
    {
        public IWebApiResponse Ok<T>(T data)
        {
            return new SuccessResponse<T>(new ApiOk<T>(data));
        }
        public IWebApiResponse Error<T>(T error)
        {
            return new FailResponse<T>(new ApiError<T>(error));
        }
    }
}
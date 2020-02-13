using NukeCore.Extensions.Http.WebApi.Models;
using NukeCore.Extensions.Http.WebApi.Models.WebApiResponse;
using NukeCore.Extensions.Monad.Response.Models;
using NukeCore.Extensions.Monad.Response.Models.Base.Interfaces;
using NukeCore.Extensions.Monad.Response.Models.Base.Resolvers;

namespace NukeCore.Extensions.Http.WebApi
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

        public IWebApiResponse CreateFrom<T>(IResponse<T> response)
        {
            if (response.IsSuccess) return new SuccessResponse<T>(new ApiOk<T>(response.Data));
            return new FailResponse<IFail>(new ApiError<IFail>(response.Error));
        }
    }
}
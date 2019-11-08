using NukeCore.Extensions.Http.Models;
using NukeCore.Extensions.Http.Models.Base.Interfaces;
using NukeCore.Extensions.Http.Models.Base.Resolvers;
using NukeCore.Extensions.Http.WebApi.Models;
using NukeCore.Extensions.Http.WebApi.Models.WebApiResponse;

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
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NukeCore.Extensions.Monad.Response.Models.Base.Interfaces;
using NukeCore.Extensions.Monad.Response.Models.Base.Resolvers;

namespace NukeCore.Extensions.Http.WebApi.Models
{
    public class FailResponse<TError> : WebApiResponse.WebApiResponse
    {
        private readonly IError<TError> _body;

        public FailResponse(IError<TError> body)
        {
            _body = body ?? new ApiError<TError>(new object());
        }

        public override string AsString()
        {
            return AsString(default);
        }

        public override string AsString(JsonSerializerSettings ss)
        {
            return JsonConvert.SerializeObject(_body.Error, ss ?? new JsonSerializerSettings());
        }

        public override OkObjectResult AsObject()
        {
            return new OkObjectResult(_body);
        }
    }
}
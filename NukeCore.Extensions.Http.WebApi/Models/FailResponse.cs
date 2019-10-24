using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UCS.Extensions.Http.Models.Base;
using UCS.Extensions.Http.Models.Base.Resolvers;

namespace UCS.Extensions.Http.WebApi.Models
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
            return JsonConvert.SerializeObject(_body.Error);
        }

        public override OkObjectResult AsObject()
        {
            return new OkObjectResult(_body.Error);
        }
    }
}
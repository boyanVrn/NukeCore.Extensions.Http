using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NukeCore.Extensions.Monad.Response.Models.Base.Interfaces;
using NukeCore.Extensions.Monad.Response.Models.Base.Resolvers;

namespace NukeCore.Extensions.Http.WebApi.Models
{
    public class SuccessResponse<TData> : WebApiResponse.WebApiResponse
    {
        private readonly IData<TData> _body;

        public SuccessResponse(IData<TData> body)
        {
            _body = body ?? new ApiOk<TData>(new object());
        }

        public override string AsString()
        {
            return AsString(default);
        }

        public override string AsString(JsonSerializerSettings ss)
        {
            return JsonConvert.SerializeObject(_body.Data, ss ?? new JsonSerializerSettings());
        }

        public override OkObjectResult AsObject()
        {
            return new OkObjectResult(_body);
        }
    }
}
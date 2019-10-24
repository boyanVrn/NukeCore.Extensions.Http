using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NukeCore.Extensions.Http.Models.Base.Interfaces;
using NukeCore.Extensions.Http.Models.Base.Resolvers;

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
            return JsonConvert.SerializeObject(_body.Data);
        }

        public override OkObjectResult AsObject()
        {
            return new OkObjectResult(_body.Data);
        }
    }
}
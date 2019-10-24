using UCS.Extensions.Http.Models.Base;

namespace UCS.Extensions.Http.Models.v1
{
    public class ApiResponseError<TData, TError> : ApiResponseOk<TData>, IError<TError>
    {
        public TError Error { get; set; }

        public ApiResponseError(TError error, TData data = default)
        {
            Data = data;
            Error = error;
        }
    }
}

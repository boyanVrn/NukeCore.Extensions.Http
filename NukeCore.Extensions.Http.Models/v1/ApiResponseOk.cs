using UCS.Extensions.Http.Models.Base;

namespace UCS.Extensions.Http.Models.v1
{
    public class ApiResponseOk<T> : IData<T>
    {
        public T Data { get; set; }

        protected ApiResponseOk() { }

        public ApiResponseOk(object data) => Data = (T)data;
    }
}

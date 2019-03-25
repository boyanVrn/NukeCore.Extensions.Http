using UCS.Extensions.Http.Models.Base;

namespace UCS.Extensions.Http.Models.v1
{
    public class ApiRequestBase<T> : IData<T>
    {
        public T Data { get; set; }

        public static ApiRequestBase<T> CreteInstance(T obj) => new ApiRequestBase<T> { Data = obj };
    }
}
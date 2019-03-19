namespace UCS.Extensions.Http.Sender.Entities
{
    public interface IDataRequest<T>
    {
        T Data { get; set; }
    }

    public class ApiRequestBase<T> : IDataRequest<T>
    {
        public T Data { get; set; }

        public static ApiRequestBase<T> CreteApiRequest(T obj) => new ApiRequestBase<T> { Data = obj };
    }
}
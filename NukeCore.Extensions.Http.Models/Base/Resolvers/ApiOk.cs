namespace UCS.Extensions.Http.Models.Base.Resolvers
{
    public class ApiOk<T> : IData<T>
    {
        public T Data { get; set; }

        protected ApiOk() { }

        public ApiOk(object data) => Data = (T)(data ?? new object());
    }
}

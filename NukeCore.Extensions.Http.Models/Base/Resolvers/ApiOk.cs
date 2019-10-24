using NukeCore.Extensions.Http.Models.Base.Interfaces;

namespace NukeCore.Extensions.Http.Models.Base.Resolvers
{
    public class ApiOk<T> : IData<T>
    {
        public T Data { get; }

        public ApiOk(object data) => Data = (T)(data ?? new object());
    }
}

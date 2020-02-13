

using NukeCore.Extensions.Monad.Response.Models.Base.Interfaces;

namespace NukeCore.Extensions.Http.Models
{
    public interface IRequest<out T> : IData<T>, ISuccess { }
}

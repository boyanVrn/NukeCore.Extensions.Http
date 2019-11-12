using NukeCore.Extensions.Http.Models.Base.Interfaces;

namespace NukeCore.Extensions.Http.Models
{
    public interface IRequest<out T> : IData<T>, ISuccess { }
}

using NukeCore.Extensions.Http.Models.Base.Interfaces;

namespace NukeCore.Extensions.Http.Models
{
    public interface IResponse<out TData> : IData<TData>, IError<IFail>, ISuccess { }
}
using NukeCore.Extensions.Http.Models.Base.Interfaces;

namespace NukeCore.Extensions.Http.Models.Factory
{
    public interface IResponseFactory
    {
        ResponseBase<TData> CreateSuccess<TData>(TData data);
        ResponseBase<TData> CreateFault<TData>(IFail error);
        ResponseBase<TData> CreateInstance<TData>(TData data, IFail error);
    }
}

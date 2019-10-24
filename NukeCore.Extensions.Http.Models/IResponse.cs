using NukeCore.Extensions.Http.Models.Base.Interfaces;

namespace NukeCore.Extensions.Http.Models
{
    public interface IResponse<out TData> : IData<TData>, IError<IFail>
    {
        bool IsSuccess { get; }
    }

    //public interface IResponseFactory
    //{
    //    IResponse<T> CreateSuccess<T>(T data);
    //    IResponse<T> CreateFault<T>(Enum code, string msg = default);
    //    IResponse<T> CreateFault<T>(IFail error);
    //}
}
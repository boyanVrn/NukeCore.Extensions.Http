using System;
using UCS.Extensions.Http.Models.Base;

namespace UCS.Extensions.Http.Models.v2
{
    public interface IResponse<TData> : IData<TData>, IError<IFail>
    {
        bool HasError { get; }
    }

    //public interface IResponseFactory
    //{
    //    IResponse<T> CreateSuccess<T>(T data);
    //    IResponse<T> CreateFault<T>(Enum code, string msg = default);
    //    IResponse<T> CreateFault<T>(IFail error);
    //}
}
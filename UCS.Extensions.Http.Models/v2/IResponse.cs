using System;
using UCS.Extensions.Http.Models.Base;

namespace UCS.Extensions.Http.Models.v2
{
    public interface IResponse<TData> : IData<TData>, IError<IFail>
    {
        bool HasError { get; }

        //IResponse<TData> CreateSuccess(TData data);
        //IResponse<TData> CreateFault(Enum code, string msg = default);
        //IResponse<TData> CreateFault(IFail error);
    }
}
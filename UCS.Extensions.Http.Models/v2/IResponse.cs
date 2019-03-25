using UCS.Extensions.Http.Models.Base;

namespace UCS.Extensions.Http.Models.v2
{
    public interface IResponse<TData, TError> : IData<TData>, IError<TError>
        where TError : class
        where TData : class
    {
        bool HasError { get; }
    }
}

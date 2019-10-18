using System;
using UCS.Extensions.Http.Models.Base;
using UCS.Extensions.Http.Models.Base.Resolvers;

namespace UCS.Extensions.Http.Models.v2
{

    public class ResponseBase<TData> : IResponse<TData>
    {

        private IFail _error;

        public IFail Error
        {
            get => _error;
            set
            {
                if (value == null) return;
                HasError = IsNotOkError(value);
                _error = value;
            }
        }

        public TData Data { get; set; }

        public bool HasError { get; private set; }

        //public IResponse<TData> CreateSuccess(TData data)
        //{
        //    return new ResponseBase<TData>(data);
        //}

        //public IResponse<TData> CreateFault(Enum code, string msg = default)
        //{
        //    return new ResponseBase<TData>(new ApiFail(code, msg));
        //}

        //public IResponse<TData> CreateFault(IFail error)
        //{
        //    return new ResponseBase<TData>(error);
        //}

        public ResponseBase() { }

        public  ResponseBase(IFail error)
        {
            Error = error;
        }
        public ResponseBase(TData data)
        {
            Data = data;
        }

        protected virtual bool IsNotOkError(IFail error) => true;
    }

    public static class ResponseFactory<TData>
    {
        //private static Lazy<ResponseBase<TData>> CreatorSingleton
        //    => new Lazy<ResponseBase<TData>>(() => new ResponseBase<TData>());

        public static ResponseBase<TData> CreateInstance(TData data) => new ResponseBase<TData>(data);//(ResponseBase<TData>)CreatorSingleton.Value.CreateSuccess(data);
        public static ResponseBase<TData> CreateInstance(IFail error) => new ResponseBase<TData>(error);
        public static ResponseBase<TData> CreateInstance(TData data, IFail error) => new ResponseBase<TData>(data) {Error = error};

    }
}

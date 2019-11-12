using NukeCore.Extensions.Http.Models.Base.Interfaces;

namespace NukeCore.Extensions.Http.Models
{

    public class ResponseBase<TData> : IResponse<TData>
    {

        private IFail _error;

        public IFail Error
        {
            get => _error;
            private set
            {
                if (value == null) return;
                IsSuccess = !IsNotOkError(value);
                _error = value;
            }
        }

        public TData Data { get; }

        public bool IsSuccess { get; private set; }

        public ResponseBase(IFail error)
        {
            Error = error;
        }
        public ResponseBase(TData data)
        {
            Data = data;
            IsSuccess = true;
        }

        public ResponseBase(TData data, IFail error)
        {
            Data = data;
            Error = error;
        }

        protected virtual bool IsNotOkError(IFail error) => true;

        //public static ResponseBase<TData> CreateSuccess(TData data) => new ResponseBase<TData>(data);
        //public static ResponseBase<TData> CreateFault(IFail error) => new ResponseBase<TData>(error);
        //public static ResponseBase<TData> CreateInstance(TData data, IFail error) => new ResponseBase<TData>(data) { Error = error };
    }
}

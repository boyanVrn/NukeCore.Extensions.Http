using UCS.Extensions.Http.Models.Base;

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

        public ResponseBase() { }

        public ResponseBase(IFail error)
        {
            Error = error;
        }
        public ResponseBase(TData data)
        {
            Data = data;
        }

        protected virtual bool IsNotOkError(IFail error) => true;

        public static ResponseBase<TData> CreateSuccess(TData data) => new ResponseBase<TData>(data);
        public static ResponseBase<TData> CreateFault(IFail error) => new ResponseBase<TData>(error);
        public static ResponseBase<TData> CreateInstance(TData data, IFail error) => new ResponseBase<TData>(data) { Error = error };
    }

}

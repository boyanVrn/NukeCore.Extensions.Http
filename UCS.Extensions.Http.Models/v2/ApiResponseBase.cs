namespace UCS.Extensions.Http.Models.v2
{

    public class ApiResponseBase<TData, TError> : IResponse<TData, TError>
        where TData : class
        where TError : class
    {
        private TError _error;

        public TError Error
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

        public ApiResponseBase() { }
        public ApiResponseBase(TError error)
        {
            Error = error;
        }
        public ApiResponseBase(TData data)
        {
            Data = data;
        }

        protected virtual bool IsNotOkError(TError error) => true;

        public static ApiResponseBase<TData, TError> CreateInstance(TData data) => new ApiResponseBase<TData, TError>(data);
        public static ApiResponseBase<TData, TError> CreateInstance(TError error) => new ApiResponseBase<TData, TError>(error);
        public static ApiResponseBase<TData, TError> CreateInstance(TData data, TError error) => new ApiResponseBase<TData, TError> { Data = data, Error = error };
    }
}

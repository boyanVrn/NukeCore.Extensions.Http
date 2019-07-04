using System;

namespace UCS.Extensions.Http.Models.Base.Resolvers
{
    public class ApiError<T> : IError<T>
    {
        public T Error { get; set; }

        public ApiError(){}

        public ApiError(object error)
        {
            if(error == null)
                throw new ArgumentNullException();

            Error = (T) error;
        }
    }
}

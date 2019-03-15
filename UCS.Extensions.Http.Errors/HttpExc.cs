using System;

namespace UCS.Extensions.Http.Errors
{
    public class HttpExc : Exception
    {
        public HttpErrorEnum ExcCode { get; }
        public HttpExc(HttpErrorEnum excCode, string message, Exception innerException = null) : base(message, innerException) => ExcCode = excCode;
    }
}

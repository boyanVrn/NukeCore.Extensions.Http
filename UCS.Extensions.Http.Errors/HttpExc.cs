using System;
using System.Net;

namespace UCS.Extensions.Http.Errors
{
    public class HttpExc : Exception
    {
        public HttpStatusCode Status { get; }

        public bool IsClientError { get; }

        public HttpExc(string message, Exception innerException = null) : base(message, innerException)
        {
            IsClientError = true;
        }

        public HttpExc(HttpStatusCode status, string message) : base(message)
        {
            Status = status;
        }

        public string BuildErrorMessage() 
            => IsClientError ? Message : $"Http exception. StatusCode: {(int)Status}. Body: {Message}";
    }
}

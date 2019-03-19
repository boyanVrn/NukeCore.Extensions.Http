using System;
using System.Net;

namespace UCS.Extensions.Http.Errors
{
    public class HttpExc : Exception
    {
        public HttpStatusCode Status { get; }
        public string Body { get; }

        public HttpExc(string message, Exception innerException = null) : base(message, innerException) { }

        public HttpExc(HttpStatusCode status, string body)
        {
            Status = status;
            Body = body;
        }

        public HttpExc(HttpStatusCode status, string body, string message, Exception innerException = null) : base(message, innerException)
        {
            Status = status;
            Body = body;
        }
    }
}

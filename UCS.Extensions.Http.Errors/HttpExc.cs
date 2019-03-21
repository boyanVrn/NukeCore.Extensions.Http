using System;
using System.Net;

namespace UCS.Extensions.Http.Errors
{

    ///<summary>Represents errors that occur during http server and client requst/response.</summary>
    public class HttpExc : Exception
    {
        public HttpStatusCode Status { get; }

        /// <summary>This is client error if flag set.</summary>
        public bool IsClientError { get; }

        /// <summary>
        /// Used in client error case. Initializes a new instance of the class HttpExc.
        /// </summary>
        /// <param name="message">Gets a message that describes the current exception.</param>
        /// <param name="innerException">An object that describes the error that caused the current exception.</param>
        public HttpExc(string message, Exception innerException = null) : base(message, innerException)
        {
            IsClientError = true;
        }

        /// <summary>
        /// Used in server error case. Initializes a new instance of the class HttpExc.
        /// </summary>
        /// <param name="status">Responsed server status code.</param>
        /// <param name="message">Gets a message that describes the current exception.</param>
        public HttpExc(HttpStatusCode status, string message) : base(message)
        {
            Status = status;
        }

        /// <summary>
        /// Creates and returns string representation of the current exception.
        /// </summary>
        /// <returns>A string representation of the current exception.</returns>
        public string BuildErrorMessage() 
            => IsClientError ? Message : $"Http exception. StatusCode: {(int)Status}. Body: {Message}";
    }
}

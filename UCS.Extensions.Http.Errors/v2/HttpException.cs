using System;
using System.Net;
using UCS.Extensions.Http.Models.Base;

namespace UCS.Extensions.Http.Errors.v2
{

    /// <inheritdoc />
    /// <summary>Represents errors that occur during http server and client requst/response.</summary>
    public class HttpException : IFail
    {
        /// <summary>
        /// Contains the value of request/responce message status code 
        /// </summary>
        public Enum Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Exception InnerException { get; }

        /// <summary>This is client error if flag set.</summary>
        public bool IsClientError { get; }

        /// <inheritdoc />
        /// <summary>
        /// Used in client error case. Initializes a new instance of the class HttpExc.
        /// </summary>
        /// <param name="message">Gets a message that describes the current exception.</param>
        /// <param name="innerException">An object that describes the error that caused the current exception.</param>
        public HttpException(string message, Exception innerException = null) 
        {
            IsClientError = true;
            InnerException = innerException;
        }

        /// <inheritdoc />
        /// <summary>
        /// Used in server error case. Initializes a new instance of the class HttpExc.
        /// </summary>
        /// <param name="status">Responsed server status code.</param>
        /// <param name="message">Gets a message that describes the current exception.</param>
        public HttpException(HttpStatusCode status, string message)
        {
            Description = message;
            Code = status;
        }

        /// <summary>
        /// Creates and returns string representation of the current exception.
        /// </summary>
        /// <returns>A string representation of the current exception.</returns>
        public string BuildErrorMessage()
            => IsClientError ? Description : $"Http exception. StatusCode: {Convert.ToInt32(Code)}. Body: {Description}";

    }

}

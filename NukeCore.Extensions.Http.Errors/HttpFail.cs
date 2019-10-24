using System;
using System.Net;
using NukeCore.Extensions.Http.Models.Base.Resolvers;

namespace NukeCore.Extensions.Http.Errors
{

    /// <inheritdoc />
    /// <summary>Represents errors that occur during http server and client requst/response.</summary>
    public class HttpFail : FailBase
    {
        /// <summary>
        /// 
        /// </summary>
        public Exception InnerException { get; }

        /// <summary>This is client error if flag set.</summary>
        
        /// <inheritdoc />
        /// <summary>
        /// Used in client error case. Initializes a new instance of the class HttpExc.
        /// </summary>
        /// <param name="message">Gets a message that describes the current exception.</param>
        /// <param name="innerException">An object that describes the error that caused the current exception.</param>
        public HttpFail(string message, Exception innerException = null) 
            : base(HttpStatusCode.InternalServerError, message)
        {
            IsInternalError = true;
            InnerException = innerException;
        }

        /// <inheritdoc />
        /// <summary>
        /// Used in server error case. Initializes a new instance of the class HttpExc.
        /// </summary>
        /// <param name="status">Responsed server status code.</param>
        /// <param name="message">Gets a message that describes the current exception.</param>
        public HttpFail(HttpStatusCode status, string message) : base(status, message) { }

        /// <summary>
        /// Creates and returns string representation of the current exception.
        /// </summary>
        /// <returns>A string representation of the current exception.</returns>
        public string BuildErrorMessage()
            => $"Http exception. StatusCode: {GetCodeAs(HttpStatusCode.OK)}. Body: {Description}";

    }

}

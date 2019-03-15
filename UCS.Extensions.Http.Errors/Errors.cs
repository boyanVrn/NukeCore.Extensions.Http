namespace UCS.Extensions.Http.Errors
{
    public enum HttpErrorEnum
    {
        ERR_OK = 200,
        ERR_HTTP_AUTH = 401,
        ERR_HTTP_ACCESS_DENIED = 403,
        ERR_HTTP_NOT_FOUND = 404,
        ERR_HTTP_CONNECTION_TIMEOUT = 408,
        ERR_HTTP_CLIENT_CANCEL_TASK = 499,
        ERR_HTTP_SERVER_INTERNAL = 500
    }
}
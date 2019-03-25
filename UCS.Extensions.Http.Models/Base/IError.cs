namespace UCS.Extensions.Http.Models.Base
{
    public interface IError<T>
    {
        T Error { get; set; }
    }
}
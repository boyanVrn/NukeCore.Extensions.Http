namespace NukeCore.Extensions.Http.Models.Base.Interfaces
{
    public interface IError<out T>
    {
        T Error { get; }
    }
}
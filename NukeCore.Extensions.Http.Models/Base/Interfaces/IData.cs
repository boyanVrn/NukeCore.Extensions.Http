namespace NukeCore.Extensions.Http.Models.Base.Interfaces
{
    public interface IData<out T>
    {
        T Data { get;}
    }
}

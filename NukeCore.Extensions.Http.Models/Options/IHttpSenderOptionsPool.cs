using NukeCore.Extensions.Http.Common.Models;

namespace NukeCore.Extensions.Http.Models.Options
{
    public interface IHttpSenderOptionsPool
    {
        void Put<T>(HttpSenderOptions options);
        void Put(string type, HttpSenderOptions options);
        HttpSenderOptions Take<T>();
        HttpSenderOptions Take(string key);
    }
}

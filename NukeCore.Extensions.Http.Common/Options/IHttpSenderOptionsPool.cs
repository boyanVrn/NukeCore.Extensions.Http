using NukeCore.Extensions.Http.Common.Settings;

namespace NukeCore.Extensions.Http.Common.Options
{
    public interface IHttpSenderOptionsPool
    {
        void Put<T>(HttpSenderOptions options);
        void Put(string type, HttpSenderOptions options);
        HttpSenderOptions Take<T>();
        HttpSenderOptions Take(string key);
    }
}

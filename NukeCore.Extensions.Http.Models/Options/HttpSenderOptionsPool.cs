using NukeCore.Extensions.Http.Common.Models;
using System.Collections.Concurrent;

namespace NukeCore.Extensions.Http.Models.Options
{
    public sealed class HttpSenderOptionsPool : ConcurrentDictionary<string, HttpSenderOptions>, IHttpSenderOptionsPool
    {
        public void Put<T>(HttpSenderOptions options)
        {
            Put(typeof(T).Name, options);
        }

        public void Put(string type, HttpSenderOptions options)
        {
            //if (ContainsKey(type))
            //    throw new ArgumentOutOfRangeException(
            //        type, $"{nameof(HttpSenderOptionsPool)}.{nameof(Put)}->Already exists in pool");

            TryAdd(type, options ?? new HttpSenderOptions());
        }

        public HttpSenderOptions Take<T>()
        {
            return Take(typeof(T).Name);
        }

        public HttpSenderOptions Take(string key)
        {
            return TryGetValue(key, out var opt) ? opt : new HttpSenderOptions();
        }

        private HttpSenderOptionsPool() { }

        public static HttpSenderOptionsPool CreateInstance() => new HttpSenderOptionsPool();

        public static HttpSenderOptionsPool CreateInstance<T>(HttpSenderOptions options) =>
                CreateInstance(typeof(T).Name, options);

        public static HttpSenderOptionsPool CreateInstance(string type, HttpSenderOptions options)
        {
            var pool = new HttpSenderOptionsPool();
            pool.Put(type, options);

            return pool;
        }
    }
}

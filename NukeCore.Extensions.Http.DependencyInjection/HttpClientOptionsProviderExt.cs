using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using NukeCore.Extensions.Http.Common.Options;
using NukeCore.Extensions.Http.Models.Settings;

namespace NukeCore.Extensions.Http.DependencyInjection
{
    internal static class HttpSenderOptionsPoolExt
    {
        internal static void PutHttpSenderOptionsInPool<T>(this IServiceCollection services, HttpSenderOptions options)
        {
            services.TryAddSingleton<IHttpSenderOptionsPool>(HttpSenderOptionsPool.CreateInstance());

            var pool = services.BuildServiceProvider().GetService<IHttpSenderOptionsPool>();
            if (pool == null) throw new ArgumentNullException();

            pool.Put<T>(options);
        }
    }
}
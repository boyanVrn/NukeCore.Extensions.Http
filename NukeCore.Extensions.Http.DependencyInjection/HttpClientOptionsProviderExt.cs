using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NukeCore.Extensions.Http.Common.Models;
using NukeCore.Extensions.Http.Models.Options;
using System;

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
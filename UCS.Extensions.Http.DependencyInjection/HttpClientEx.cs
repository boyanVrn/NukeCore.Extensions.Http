using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace UCS.Extensions.Http.DependencyInjection
{

    public static class HttpClientExtension
    {
        public static IServiceCollection AddConfiguredHttpClient<TClient, TImplementation, THttpClientHandler>(this IServiceCollection services, Action<HttpClientOptionsProvider, THttpClientHandler> cfgAction)
            where TClient : class
            where TImplementation : class, TClient
            where THttpClientHandler : HttpClientHandler, new()
        {
            if (cfgAction == null)
                throw new NullReferenceException();

            var cfg = new HttpClientOptionsProvider();
            var handler = new THttpClientHandler();
           
            cfgAction.Invoke(cfg, handler);

            if (cfg.BaseAddress == null)
                throw new NullReferenceException();

            return services
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient(
                    (sp, options) =>
                    {
                        options.BaseAddress = cfg.BaseAddress;
                        cfg.DefaultRequestHeaders.ForEach(h => options.DefaultRequestHeaders.Accept.Add(h));
                    })
                .ConfigurePrimaryHttpMessageHandler(x => handler)
                .Services;
        }
    }
}

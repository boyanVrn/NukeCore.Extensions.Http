using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace UCS.Extensions.Http.DependencyInjection
{

    public static class HttpClientExtension
    {
        public static IServiceCollection AddConfiguredHttpClient<TClient, TImplementation>(this IServiceCollection services, Action<HttpClientOptionsProvider> cfgAction)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (cfgAction == null)
                throw new ArgumentNullException();

            var cfg = new HttpClientOptionsProvider();

            cfgAction.Invoke(cfg);

            if (cfg.BaseAddress == null)
                throw new ArgumentNullException();

            return services
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient(
                    (sp, options) =>
                    {
                        options.BaseAddress = cfg.BaseAddress;
                        options.Timeout = cfg.Timeout;
                        cfg.DefaultRequestHeaders.ForEach(h => options.DefaultRequestHeaders.Accept.Add(h));
                    })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler
                    {
                        UseProxy = cfg.HasProxy,
                        Credentials = cfg.Credentials,
                        AutomaticDecompression = cfg.ResponseAutoDecompressionType
                    };

                    if (!cfg.HasServerCertificateValidation) handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                    return handler;
                })
                .Services;
        }
    }
}

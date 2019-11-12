using System;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NukeCore.Extensions.Http.Models.Factory;

namespace NukeCore.Extensions.Http.DependencyInjection
{

    /// <summary>
    /// Custom http client extended methods repository
    /// </summary>
    public static class HttpClientExtension
    {
        /// <summary>
        /// register new configured http client factory to host services
        /// </summary>
        /// <param name="services">host services list</param>
        /// <param name="cfgAction">HttpClientOptionsProvider action</param>
        /// <typeparam name="TClient">Sender class type</typeparam>
        /// <typeparam name="TImplementation">Sender implementation interface</typeparam>
        /// <exception cref="ArgumentNullException">wrong configuration params</exception>
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

            services.AddSingleton(cfg.SenderOptions);
            services.TryAddSingleton<IResponseFactory, ResponseFactory>();
   
            return services
                .AddHttpClient<TClient, TImplementation>()
                .ConfigureHttpClient(
                    (sp, options) =>
                    {
                        options.BaseAddress = cfg.BaseAddress;
                        options.Timeout = Timeout.InfiniteTimeSpan; //override by HttpClientOptionsProvider
                        cfg.AcceptHeaders.ForEach(h => options.DefaultRequestHeaders.Accept.Add(h));
                        cfg.RequestHeaders.CopyTo(options.DefaultRequestHeaders);
                    })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler
                    {
                        UseProxy = cfg.HasProxy,
                        Credentials = cfg.Credentials,
                        AutomaticDecompression = cfg.ResponseAutoDecompressionType
                    };

                    if (!cfg.HasServerCertificateValidation)
                        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                    return handler;
                })
                .Services;
        }
    }
}

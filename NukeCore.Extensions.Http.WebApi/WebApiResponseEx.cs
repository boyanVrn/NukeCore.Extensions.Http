using Microsoft.Extensions.DependencyInjection;

namespace NukeCore.Extensions.Http.WebApi
{

    /// <summary>
    /// Web api factory extended methods repository
    /// </summary>
    public static class WebApiResponseEx
    {
        /// <summary>
        /// register new ioc container for web api response factory
        /// </summary>
        /// <param name="services">host services list</param>

        public static IServiceCollection AddWebApiResponseFactory(this IServiceCollection services)
        {
            return services.AddScoped<IWebApiResponseFactory, WebApiResponseFactory>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Queue.Client.Abstract;
using Soenneker.Utils.HttpClientCache.Registrar;

namespace Soenneker.Queue.Client.Registrars;

/// <summary>
/// Registers Azure Queue Storage client caching services.
/// </summary>
public static class QueueClientUtilRegistrar
{
    /// <summary>
    /// Registers one queue-client utility and queue-client cache for the application.
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddQueueClientUtilAsSingleton(this IServiceCollection services)
    {
        services.AddHttpClientCacheAsSingleton().TryAddSingleton<IQueueClientUtil, QueueClientUtil>();

        return services;
    }

    /// <summary>
    /// Registers one queue-client utility per dependency-injection scope while retaining the singleton HTTP transport cache.
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddQueueClientUtilAsScoped(this IServiceCollection services)
    {
        services.AddHttpClientCacheAsSingleton().TryAddScoped<IQueueClientUtil, QueueClientUtil>();

        return services;
    }
}

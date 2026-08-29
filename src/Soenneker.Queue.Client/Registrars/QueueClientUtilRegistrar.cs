using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Queue.Client.Abstract;
using Soenneker.Utils.HttpClientCache.Registrar;

namespace Soenneker.Queue.Client.Registrars;

/// <summary>
/// A utility library for Azure Queue (Storage) client accessibility
/// </summary>
public static class QueueClientUtilRegistrar
{
    /// <summary>
    /// Recommended
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddQueueClientUtilAsSingleton(this IServiceCollection services)
    {
        services.AddHttpClientCacheAsSingleton().TryAddSingleton<IQueueClientUtil, QueueClientUtil>();

        return services;
    }

    /// <summary>
    /// Registers Queue Client Util with a scoped lifetime.
    /// </summary>
    /// <param name="services">Service collection that receives the registration.</param>
    /// <returns>The same service collection, so additional registrations can be chained.</returns>
    public static IServiceCollection AddQueueClientUtilAsScoped(this IServiceCollection services)
    {
        services.AddHttpClientCacheAsSingleton().TryAddScoped<IQueueClientUtil, QueueClientUtil>();

        return services;
    }
}

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
    public static IServiceCollection AddQueueClientUtilAsSingleton(this IServiceCollection services)
    {
        services.AddHttpClientCacheAsSingleton().TryAddSingleton<IQueueClientUtil, QueueClientUtil>();

        return services;
    }

    public static IServiceCollection AddQueueClientUtilAsScoped(this IServiceCollection services)
    {
        services.AddHttpClientCacheAsSingleton().TryAddScoped<IQueueClientUtil, QueueClientUtil>();

        return services;
    }
}
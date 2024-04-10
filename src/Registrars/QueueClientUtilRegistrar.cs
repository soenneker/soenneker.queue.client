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
    public static void AddQueueClientUtilAsSingleton(this IServiceCollection services)
    {
        services.AddHttpClientCache();
        services.TryAddSingleton<IQueueClientUtil, QueueClientUtil>();
    }

    public static void AddQueueClientUtilAsScoped(this IServiceCollection services)
    {
        services.AddHttpClientCache();
        services.TryAddScoped<IQueueClientUtil, QueueClientUtil>();
    }
}

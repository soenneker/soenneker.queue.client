using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.Configuration;
using Soenneker.Extensions.String;
using Soenneker.Extensions.Task;
using Soenneker.Extensions.ValueTask;
using Soenneker.Queue.Client.Abstract;
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Utils.HttpClientCache.Abstract;
using Soenneker.Utils.SingletonDictionary;

namespace Soenneker.Queue.Client;

///<inheritdoc cref="IQueueClientUtil"/>
public sealed class QueueClientUtil : IQueueClientUtil
{
    private const string _httpClientKey = nameof(QueueClientUtil);

    private readonly ILogger<QueueClientUtil> _logger;
    private readonly IHttpClientCache _httpClientCache;

    private readonly string _connectionString;

    // Built once; reused for every QueueClient.
    private readonly AsyncSingleton<QueueClientOptions> _queueClientOptions;

    private readonly SingletonDictionary<QueueClient> _queueClients;

    public QueueClientUtil(IConfiguration config, IHttpClientCache httpClientCache, ILogger<QueueClientUtil> logger)
    {
        _logger = logger;
        _httpClientCache = httpClientCache;

        // Sync lookup once (and avoids repeating config reads per queue)
        _connectionString = config.GetValueStrict<string>("Azure:Storage:Queue:ConnectionString");

        // No closure: method group
        _queueClientOptions = new AsyncSingleton<QueueClientOptions>(CreateQueueClientOptions);

        // No closure: method group
        _queueClients = new SingletonDictionary<QueueClient>(CreateQueueClient);
    }

    private async ValueTask<QueueClientOptions> CreateQueueClientOptions(CancellationToken token)
    {
        HttpClient httpClient = await _httpClientCache.Get(_httpClientKey, cancellationToken: token)
                                                      .NoSync();

        // Allocate these ONCE.
        return new QueueClientOptions
        {
            Transport = new HttpClientTransport(httpClient)
        };
    }

    private async ValueTask<QueueClient> CreateQueueClient(string queueName, CancellationToken token)
    {
        // Reused; no per-queue allocations for options/transport anymore.
        QueueClientOptions options = await _queueClientOptions.Get(token)
                                                              .NoSync();

        var queueClient = new QueueClient(_connectionString, queueName, options);

        if (await queueClient.ExistsAsync(token)
                             .NoSync())
            return queueClient;

        _logger.LogInformation("Queue did not exist, so creating: {queue}", queueName);

        await queueClient.CreateAsync(cancellationToken: token)
                         .NoSync();

        return queueClient;
    }

    public ValueTask<QueueClient> Get(string queue, CancellationToken cancellationToken = default)
    {
        string queueLowered = queue.ToLowerInvariantFast();
        return _queueClients.Get(queueLowered, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _queueClients.DisposeAsync()
                           .NoSync();
        await _queueClientOptions.DisposeAsync()
                                 .NoSync();
        await _httpClientCache.Remove(_httpClientKey)
                              .NoSync();
    }

    public void Dispose()
    {
        _queueClients.Dispose();
        _queueClientOptions.Dispose();
        _httpClientCache.RemoveSync(_httpClientKey);
    }
}
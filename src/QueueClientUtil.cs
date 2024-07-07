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
using Soenneker.Utils.HttpClientCache.Abstract;
using Soenneker.Utils.SingletonDictionary;

namespace Soenneker.Queue.Client;

///<inheritdoc cref="IQueueClientUtil"/>
public class QueueClientUtil : IQueueClientUtil
{
    private readonly IHttpClientCache _httpClientCache;
    private readonly SingletonDictionary<QueueClient> _queueClients;

    public QueueClientUtil(IConfiguration config, IHttpClientCache httpClientCache, ILogger<QueueClientUtil> logger)
    {
        _httpClientCache = httpClientCache;
        _queueClients = new SingletonDictionary<QueueClient>( async (queueName, token, _) =>
        {
            var connectionString = config.GetValueStrict<string>("Azure:Storage:Queue:ConnectionString");

            HttpClient httpClient = await _httpClientCache.Get(nameof(QueueClientUtil), cancellationToken: token).NoSync();

            var clientOptions = new QueueClientOptions
            {
                Transport = new HttpClientTransport(httpClient)
            };

            var queueClient = new QueueClient(connectionString, queueName, clientOptions);

            if (await queueClient.ExistsAsync(token).NoSync())
                return queueClient;

            logger.LogInformation("Queue did not exist, so creating: {queue}", queueName);
            await queueClient.CreateAsync(cancellationToken: token).NoSync();

            return queueClient;
        });
    }

    public ValueTask<QueueClient> Get(string queue, CancellationToken cancellationToken = default)
    {
        string queueLowered = queue.ToLowerInvariantFast();

        return _queueClients.Get(queueLowered, cancellationToken, queueLowered);
    }
    
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await _httpClientCache.Remove(nameof(QueueClientUtil)).NoSync();

        await _queueClients.DisposeAsync().NoSync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _httpClientCache.RemoveSync(nameof(QueueClientUtil));

        _queueClients.Dispose();
    }
}
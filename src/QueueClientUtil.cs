using System;
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
        _queueClients = new SingletonDictionary<QueueClient>( async args =>
        {
            var connectionString = config.GetValueStrict<string>("Azure:Storage:Queue:ConnectionString");

            var queueName = (string) args![0];

            var clientOptions = new QueueClientOptions
            {
                Transport = new HttpClientTransport(await _httpClientCache.Get(nameof(QueueClientUtil)).NoSync())
            };

            var queueClient = new QueueClient(connectionString, queueName, clientOptions);

            if (await queueClient.ExistsAsync().NoSync())
                return queueClient;

            logger.LogInformation("Queue did not exist, so creating: {queue}", queueName);
            await queueClient.CreateAsync().NoSync();

            return queueClient;
        });
    }

    public ValueTask<QueueClient> Get(string queue)
    {
        string queueLowered = queue.ToLowerInvariantFast();

        return _queueClients.Get(queueLowered, queueLowered);
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
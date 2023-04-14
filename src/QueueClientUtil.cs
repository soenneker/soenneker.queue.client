using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Extensions.Configuration;
using Soenneker.Queue.Client.Abstract;
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Utils.SingletonDictionary;

namespace Soenneker.Queue.Client;

///<inheritdoc cref="IQueueClientUtil"/>
public class QueueClientUtil : IQueueClientUtil
{
    private readonly AsyncSingleton<HttpClient> _httpClient;

    private readonly SingletonDictionary<QueueClient> _queueClients;

    public QueueClientUtil(IConfiguration config, ILogger<QueueClientUtil> logger)
    {
        _httpClient = new AsyncSingleton<HttpClient>(() =>
        {
            var socketsHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                MaxConnectionsPerServer = 20
            };

            var httpClient = new HttpClient(socketsHandler);
            httpClient.Timeout = TimeSpan.FromSeconds(30); // TODO: Research good timeout of queue client

            return httpClient;
        });

        _queueClients = new SingletonDictionary<QueueClient>( async (args) =>
        {
            var queueName = (string) args![0];

            var clientOptions = new QueueClientOptions
            {
                Transport = new HttpClientTransport(await _httpClient.Get())
            };

            var connectionString = config.GetValueStrict<string>("Azure:Storage:Queue:ConnectionString");

            var queueClient = new QueueClient(connectionString, queueName, clientOptions);

            if (await queueClient.ExistsAsync())
                return queueClient;

            logger.LogInformation("Queue did not exist, so creating: {queue}", queueName);
            await queueClient.CreateAsync();

            return queueClient;
        });
    }

    public ValueTask<QueueClient> GetClient(string queue)
    {
        string queueLowered = queue.ToLowerInvariant();

        return _queueClients.Get(queueLowered, queueLowered);
    }
    
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        await _queueClients.DisposeAsync();

        await _httpClient.DisposeAsync();
    }

    public void Dispose()
    {
        _queueClients.Dispose();

        _httpClient.Dispose();
    }
}
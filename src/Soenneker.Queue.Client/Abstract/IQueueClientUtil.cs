using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;

namespace Soenneker.Queue.Client.Abstract;

/// <summary>
/// Provides cached Azure Queue Storage clients and creates queues when necessary.
/// </summary>
public interface IQueueClientUtil : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Gets a client for the specified queue, creating the queue if it does not exist.
    /// </summary>
    /// <param name="queue">The queue name. It is normalized to lowercase before lookup.</param>
    /// <param name="cancellationToken">A token to cancel queue creation or lookup.</param>
    /// <returns>The cached client for the normalized queue name.</returns>
    [Pure]
    ValueTask<QueueClient> Get(string queue, CancellationToken cancellationToken = default);
}

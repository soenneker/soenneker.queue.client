using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;

namespace Soenneker.Queue.Client.Abstract;

/// <summary>
/// A utility library for Azure Queue (Storage) client accessibility <para/>
/// Singleton IoC recommended
/// </summary>
public interface IQueueClientUtil : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Returns the configured queue Client used by the Queue Client.
    /// </summary>
    /// <param name="queue">Queue for the get operation.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the requested queue Client.</returns>
    [Pure]
    ValueTask<QueueClient> Get(string queue, CancellationToken cancellationToken = default);
}

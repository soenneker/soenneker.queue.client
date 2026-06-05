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
    /// Gets the value.
    /// </summary>
    /// <param name="queue">The queue.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    [Pure]
    ValueTask<QueueClient> Get(string queue, CancellationToken cancellationToken = default);
}
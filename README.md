[![](https://img.shields.io/nuget/v/Soenneker.Queue.Client.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Queue.Client/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.queue.client/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.queue.client/actions/workflows/publish-package.yml)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.queue.client/build-and-test.yml?label=Build&style=for-the-badge)](https://github.com/soenneker/soenneker.queue.client/actions/workflows/build-and-test.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Queue.Client.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Queue.Client/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.queue.client/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.queue.client/actions/workflows/codeql.yml)

# Soenneker.Queue.Client

Provides cached Azure Queue Storage `QueueClient` instances through dependency injection and creates a requested queue when it does not exist.

## Install

```bash
dotnet add package Soenneker.Queue.Client
```

## Configuration

```json
{
  "Azure": {
    "Storage": {
      "Queue": {
        "ConnectionString": "<Azure Storage connection string>"
      }
    }
  }
}
```

The connection string is read when `QueueClientUtil` is constructed. The credential must be allowed to inspect and create queues in addition to performing the application’s queue operations.

## Registration

```csharp
using Soenneker.Queue.Client.Registrars;

builder.Services.AddQueueClientUtilAsSingleton();
```

Singleton registration shares the per-queue client cache across the application. Scoped registration is also available:

```csharp
builder.Services.AddQueueClientUtilAsScoped();
```

With scoped registration, each scope owns its queue-client lookup cache, while the underlying cached HTTP transport remains singleton-owned and survives disposal of individual scopes.

Both registration methods use `TryAdd`; an existing `IQueueClientUtil` registration is preserved.

## Usage

```csharp
using Azure.Storage.Queues;
using Soenneker.Queue.Client.Abstract;

public sealed class WorkPublisher(IQueueClientUtil queueClients)
{
    public async ValueTask Publish(string json, CancellationToken cancellationToken)
    {
        QueueClient queue = await queueClients.Get("work-items", cancellationToken);
        await queue.SendMessageAsync(json, cancellationToken);
    }
}
```

`Get` normalizes the queue name to lowercase, creates the queue if necessary, and caches the resulting `QueueClient` for the lifetime of the utility. Azure Queue Storage naming rules still apply.

Disposing a scoped utility releases its own cached queue objects but does not evict the shared HTTP client. The DI container owns and disposes the singleton HTTP cache at application shutdown.

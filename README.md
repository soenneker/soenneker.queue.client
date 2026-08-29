[![](https://img.shields.io/nuget/v/Soenneker.Queue.Client.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Queue.Client/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.queue.client/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.queue.client/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Queue.Client.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Queue.Client/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.queue.client/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.queue.client/actions/workflows/codeql.yml)

# Soenneker.Queue.Client

A utility library for Azure Queue (Storage) client accessibility Singleton IoC recommended.

## Install

```bash
dotnet add package Soenneker.Queue.Client
```

## Quick start

```csharp
using Soenneker.Queue.Client.Registrars;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var result = services.AddQueueClientUtilAsSingleton();
```

Recommended.

## What you get

- `IQueueClientUtil` — A utility library for Azure Queue (Storage) client accessibility Singleton IoC recommended.
- `QueueClientUtilRegistrar` — A utility library for Azure Queue (Storage) client accessibility.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `QueueClientUtilRegistrar.AddQueueClientUtilAsSingleton(services)` | Recommended. | The same service collection, so additional registrations can be chained. |
| `QueueClientUtilRegistrar.AddQueueClientUtilAsScoped(services)` | Registers Queue Client Util with a scoped lifetime. | The same service collection, so additional registrations can be chained. |

## Practical notes

- Reuse the registered client instead of constructing one per operation.
- Calls that return a cached or singleton value reuse the same instance until the owning service is disposed.
- Dispose instances you own when their scope ends so held resources can be released.

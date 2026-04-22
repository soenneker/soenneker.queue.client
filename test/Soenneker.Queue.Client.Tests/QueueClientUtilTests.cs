using Soenneker.Queue.Client.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Queue.Client.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class QueueClientUtilTests : HostedUnitTest
{
    private readonly IQueueClientUtil _util;

    public QueueClientUtilTests(Host host) : base(host)
    {
        _util = Resolve<IQueueClientUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}

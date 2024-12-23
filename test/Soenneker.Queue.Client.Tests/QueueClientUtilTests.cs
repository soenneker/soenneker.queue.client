using Soenneker.Queue.Client.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Queue.Client.Tests;

[Collection("Collection")]
public class QueueClientUtilTests : FixturedUnitTest
{
    private readonly IQueueClientUtil _util;

    public QueueClientUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IQueueClientUtil>(true);
    }

    [Fact]
    public void Default()
    {

    }
}

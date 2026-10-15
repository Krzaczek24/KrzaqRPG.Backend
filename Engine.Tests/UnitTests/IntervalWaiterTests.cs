using Krzaq.Extensions.IEnumerable;
using KrzaqRPG.Backend.Engine.Core.Waiter;

namespace KrzaqRPG.Backend.Engine.Tests.UnitTests;

internal class IntervalWaiterTests
{
    [Test]
    public async Task Test()
    {
        // --- Arrange ---
        const int testForNseconds = 3;
        const double intervalSeconds = 0.25;
        var interval = TimeSpan.FromSeconds(intervalSeconds);
        var testUntil = DateTime.Now.AddSeconds(testForNseconds);

        var timestamps = new List<DateTime>();
        var expectedResults = Enumerable
            .Range(1, (int)(testForNseconds / intervalSeconds) - 1)
            .Select(_ => interval)
            .ToList();

        var waiter = new IntervalWaiter(interval);

        // --- Act ---
        while (DateTime.Now < testUntil)
        {
            await waiter.AwaitAsync();
            timestamps.Add(DateTime.Now);
        }
        var actualResults = timestamps
            .SlidingWindow(2)
            .Select(window => window[1] - window[0])
            .Select(diff => TimeSpan.FromSeconds(Round(diff.TotalSeconds, 0.05)))
            .ToList();

        // --- Assert ---

        Assert.That(actualResults, Is.EqualTo(expectedResults));
    }

    private static double Round(double value, double multipleOf)
    {
        return (double)Math.Round((decimal)value / (decimal)multipleOf, MidpointRounding.AwayFromZero) * multipleOf;
    }
}
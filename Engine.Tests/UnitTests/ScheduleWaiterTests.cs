using KrzaqRPG.Backend.Engine.Core.Waiter;
using Microsoft.Extensions.Time.Testing;
using NCrontab;

namespace KrzaqRPG.Backend.Engine.Tests.UnitTests;

internal class ScheduleWaiterTests
{
    [Test]
    public async Task Test()
    {
        // --- Arrange ---
        const int testForNseconds = 3;

        var testDate = DateTime.Today;
        var testUntil = testDate.AddSeconds(testForNseconds);

        var actualResults = new List<DateTime>();
        var expectedResults = Enumerable.Range(0, testForNseconds + 1).Select(x => testDate.AddSeconds(x)).ToList();

        var fakeTimeProvider = new FakeTimeProvider(new DateTimeOffset(testDate).AddSeconds(-1));
        var waiter = new ScheduleWaiter(
            CrontabSchedule.Parse($"* * * * * *", new() { IncludingSeconds = true }),
            fakeTimeProvider);

        // --- Act ---
        while (fakeTimeProvider.GetLocalNow() < testUntil)
        {
            var delay = waiter.Delay;
            await waiter.AwaitAsync();
            fakeTimeProvider.Advance(delay);
            actualResults.Add(fakeTimeProvider.GetLocalNow().DateTime);
        }

        // --- Assert ---
        Assert.That(actualResults, Is.EqualTo(expectedResults));
    }
}
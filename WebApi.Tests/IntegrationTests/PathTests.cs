using KrzaqRPG.Backend.WebApi.Controllers;
using KrzaqRPG.WebApi.Tests.IntegrationTests.Base;

namespace KrzaqRPG.WebApi.Tests.IntegrationTests;

internal class PathTests : IntegrationTestBase
{
    protected AreasController Controller { get; set; }

    [OneTimeSetUp]
    public override void OneTimeSetUp()
    {
        base.OneTimeSetUp();
        Controller = new AreasController(Mediator);
    }

    private static IEnumerable<TestCaseData> TravelAreasCases()
    {
        yield return new TestCaseData(new List<string>
        {
            "Ignaroth", "Ignaroth Wilderness", "Western Outpost", "Pinterthon",
            "Faramh Woods", "The Rustling Forest", "Hoghmith", "Southern Outpost",
            "The Broken Arrow Inn", "Midlake", "Midlake Causeway", "Laram Exeteris",
            "Gordium", "Finingham", "Calderas", "Calderas Glade", "Valderum Woods",
            "Hunters Post", "At the Full Mug Tavern", "Traders' Encampment",
            "Northern Outpost", "Faramh Wilderness", "Faramh"
        }).SetName($"{nameof(TryTravelingFromAreaToAreaTest)}_LongRoute");
    }

    [Test]
    [TestCaseSource(nameof(TravelAreasCases))]
    public async Task TryTravelingFromAreaToAreaTest(IList<string> travelAreas)
    {
        // --- Arrange ---
        var availableAreas = (await Controller.GetList(default)).Value!.Areas;

        if (IsAnyAreaUnavailable(availableAreas.Select(a => a.Name), travelAreas, out string? unavailableArea))
        {
            Assert.Fail($"Area '{unavailableArea}' is unavailable!");
        }

        var travelAreasIterator = travelAreas.GetEnumerator();
        travelAreasIterator.MoveNext();

        var currentArea = availableAreas.First(a => a.Name == travelAreasIterator.Current);
        var endArea = availableAreas.FirstOrDefault(x => x.Name == travelAreas[^1]);

        // --- Act ---
        while (travelAreasIterator.MoveNext())
        {
            var currentAreaAvailablePaths = (await Controller.GetAvailablePaths(currentArea.Id, default)).Value!.AvailablePaths;
            var nextArea = currentAreaAvailablePaths.Select(x => x.TargetArea).FirstOrDefault(x => x.Name == travelAreasIterator.Current);
            currentArea = availableAreas.FirstOrDefault(x => x.Id == (nextArea?.Id ?? default));
            if (currentArea is null)
                break;
        }

        // --- Assert ---
        Assert.That(currentArea, Is.EqualTo(endArea));
    }

    private static bool IsAnyAreaUnavailable(
        IEnumerable<string> availableAreas,
        IEnumerable<string> travelAreas,
        out string? unavailableArea)
    {
        unavailableArea = travelAreas.FirstOrDefault(area => !availableAreas.Contains(area));
        return unavailableArea is not null;
    }
}

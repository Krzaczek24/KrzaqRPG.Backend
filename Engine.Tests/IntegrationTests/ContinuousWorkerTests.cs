using RpgAdventureGame.Backend.Database.SQLite.Base;
using RpgAdventureGame.Backend.Database.SQLite.Entities.EngineWorkerJob;
using RpgAdventureGame.Backend.Engine.Tests.IntegrationTests.Base;

namespace RpgAdventureGame.Backend.Engine.Tests.IntegrationTests
{
    internal class ContinuousWorkerTests : IntegrationTestBase
    {
        protected override async Task AddEntities(List<DbTable> entities)
        {
            entities.Add(new DbEngineWorkerJob
            {
                
            });
        }

        [Test]
        public void Test1()
        {
            
        }
    }
}

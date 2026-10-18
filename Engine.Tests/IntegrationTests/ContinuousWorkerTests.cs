using KrzaqRPG.Backend.Database.SQLite.Base;
using KrzaqRPG.Backend.Database.SQLite.Entities.EngineWorkerJobs;
using KrzaqRPG.Backend.Engine.Tests.IntegrationTests.Base;

namespace KrzaqRPG.Backend.Engine.Tests.IntegrationTests
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

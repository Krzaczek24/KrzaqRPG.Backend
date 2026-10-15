using KrzaqRPG.Backend.Database.SQLite;
using KrzaqRPG.Backend.Database.SQLite.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KrzaqRPG.Backend.Engine.Tests.IntegrationTests.Base
{
    [NonParallelizable]
    internal abstract class IntegrationTestBase
    {
        private string testDbFilePath = string.Empty;

        private ServiceProvider? provider;
        protected IServiceProvider Provider => provider!;

        [OneTimeSetUp]
        public void BuildEnvironment()
        {
            testDbFilePath = $"TestDb_{Guid.NewGuid():N}.sqlite";

            var services = new ServiceCollection();

            services.AddDbContext<Db>(opts => opts.UseSqlite($"Data Source={testDbFilePath}"));

            services.AddAppDbAccesses();

            AddServices(services);

            provider = services.BuildServiceProvider();
        }

        [SetUp]
        public async Task InitializeDatabaseContent()
        {
            await using var scope = provider!.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<Db>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
            var entities = new List<DbTable>();
            await AddEntities(entities);
            await db.AddRangeAsync(entities);
        }

        protected virtual void AddServices(IServiceCollection services)
        {

        }

        protected abstract Task AddEntities(List<DbTable> entities);

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            provider?.Dispose();

            if (!string.IsNullOrEmpty(testDbFilePath)
            && File.Exists(testDbFilePath))
            {
                try { File.Delete(testDbFilePath); } catch { }
            }
        }
    }
}

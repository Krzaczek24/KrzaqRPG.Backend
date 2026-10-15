using Krzaq.MediatR;
using Krzaq.MediatR.Implementations;
using Krzaq.MediatR.Interfaces;
using KrzaqRPG.Backend.Database.SQLite;
using KrzaqRPG.Backend.Database.SQLite;
using KrzaqRPG.Backend.WebApi;
using KrzaqRPG.Backend.WebApi.Core.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace KrzaqRPG.WebApi.Tests.IntegrationTests.Base
{
    [NonParallelizable]
    internal abstract class IntegrationTestBase
    {
        private const string RealDbFilePath = @"E:\KrzaqRPG\KrzaqRPG.SQLite.db";
        private string testDbFilePath = string.Empty;

        private ServiceProvider? provider;
        protected IServiceProvider Provider => provider!;

        protected IMediator Mediator => Provider.GetRequiredService<IMediator>();

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            if (!File.Exists(RealDbFilePath))
                throw new FileNotFoundException("Real DB file not found", RealDbFilePath);

            testDbFilePath = $"TestDb_{Guid.NewGuid():N}.sqlite";

            CopyDatabaseFile();

            var services = new ServiceCollection();

            services.AddDbContext<Db>(opts => opts.UseSqlite($"Data Source={testDbFilePath}"));

            services.AddAppDbAccesses();

            services.AddMediator(typeof(Program).Assembly);
            services.AddHandlers(typeof(Program).Assembly);

            services.AddSingleton<IRequestErrorsHandler, ErrorHandler>();

            provider = services.BuildServiceProvider();
        }

        [SetUp]
        public virtual void SetUp() => CopyDatabaseFile();

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

        private void CopyDatabaseFile() => File.Copy(RealDbFilePath, testDbFilePath, true);
    }
}

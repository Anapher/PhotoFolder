using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using PhotoFolder.Core;
using PhotoFolder.Infrastructure;
using PhotoFolder.Infrastructure.Data;
using PhotoFolder.Infrastructure.Photos;
using Serilog;
using Xunit.Abstractions;

namespace PhotoFolder.Application.IntegrationTests
{
    public class ApplicationContext
    {
        public ApplicationContext(IContainer container, SqliteConnection sqliteConnection, MockFileSystem mockFileSystem)
        {
            Container = container;
            SqliteConnection = sqliteConnection;
            MockFileSystem = mockFileSystem;
        }

        public IContainer Container { get; }
        public SqliteConnection SqliteConnection { get; }
        public MockFileSystem MockFileSystem { get; }

        public void AddResourceFile(string path, string resourceName)
        {
            MockFileSystem.AddFileFromEmbeddedResource(path, Assembly.GetExecutingAssembly(),
                $"PhotoFolder.Application.IntegrationTests.Resources.{resourceName}");
        }

        public static ApplicationContext Initialize(ITestOutputHelper testOutput)
        {
            var fileSystem = new MockFileSystem();

            var builder = new ContainerBuilder();
            builder.RegisterModule<CoreModule>();
            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterModule<ApplicationModule>();

            builder.RegisterInstance(fileSystem).As<IFileSystem>();

            builder.RegisterInstance(Options.Create(new WorkspaceOptions {ApplyMigrations = false})).As<IOptions<WorkspaceOptions>>();
            builder.RegisterInstance(Options.Create(new BitmapHashOptions())).As<IOptions<BitmapHashOptions>>();

            var services = new ServiceCollection();

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(testOutput, Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger()
                .ForContext<ApplicationContext>();

            services.AddLogging(loggingBuilder =>
              loggingBuilder.AddSerilog(logger, dispose: true));

            builder.Populate(services);

            var dbName = Guid.NewGuid().ToString("N");
            var connectionString = $"DataSource={dbName};mode=memory;cache=shared";
            var connection = new SqliteConnection(connectionString);
            connection.Open();

            var mockDbContextBuilder = new Mock<IAppDbContextOptionsBuilder>();
            var contextBuilder = new DbContextOptionsBuilder<AppDbContext>();
            contextBuilder.UseSqlite(connectionString);
            mockDbContextBuilder.Setup(x => x.Build(It.IsAny<string>())).Returns(contextBuilder.Options);

            builder.RegisterInstance(mockDbContextBuilder.Object).As<IAppDbContextOptionsBuilder>();

            builder.RegisterType<AutofacServiceProvider>().AsImplementedInterfaces();

            return new ApplicationContext(builder.Build(), connection, fileSystem);
        }
    }
}

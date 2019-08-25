﻿using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using PhotoFolder.Core;
using PhotoFolder.Infrastructure;
using PhotoFolder.Infrastructure.Data;
using PhotoFolder.Infrastructure.Photos;

namespace PhotoFolder.Application.IntegrationTests
{
    public class AppContext
    {
        public AppContext(IContainer container, SqliteConnection sqliteConnection, MockFileSystem mockFileSystem)
        {
            Container = container;
            SqliteConnection = sqliteConnection;
            MockFileSystem = mockFileSystem;
        }

        public IContainer Container { get; }
        public SqliteConnection SqliteConnection { get; }
        public MockFileSystem MockFileSystem { get; }

        public static AppContext Initialize()
        {
            var fileSystem = new MockFileSystem();

            var builder = new ContainerBuilder();
            builder.RegisterModule<CoreModule>();
            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterModule<ApplicationModule>();

            builder.RegisterInstance(fileSystem).As<IFileSystem>();

            builder.RegisterInstance(Options.Create(new WorkspaceOptions {ApplyMigrations = false})).As<IOptions<WorkspaceOptions>>();
            builder.RegisterInstance(Options.Create(new BitmapHashOptions())).As<IOptions<BitmapHashOptions>>();

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

            return new AppContext(builder.Build(), connection, fileSystem);
        }
    }
}

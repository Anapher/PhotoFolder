using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using PhotoFolder.Application.Dto.WorkerRequests;
using PhotoFolder.Application.Interfaces.Workers;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Infrastructure.Photos;
using PhotoFolder.Infrastructure.Services;
using Xunit;
using Xunit.Abstractions;

namespace PhotoFolder.Application.IntegrationTests
{
    public static class DefaultPhotoFolder
    {
        public const string PhotoFolderPath = @"C:\Users\Vincent\Photos\Photo Folder";

        public static readonly IImmutableDictionary<string, string> DefaultFileBase =
            new[] {"egypt_sonyz3.jpg", "flora_sonyalpha.jpg", "hanszimmer_htcu11.jpg"}.ToImmutableDictionary(x => x, x => x);

        public static readonly IImmutableDictionary<string, string> CleanFileBase = new Dictionary<string, string>
        {
            {"2019/03.30/hanszimmer_htcu11.jpg", "hanszimmer_htcu11.jpg"},
            {"2015/10.25/egypt_sonyz3.jpg", "egypt_sonyz3.jpg"},
            {"2010/05.09/flora_sonyalpha.jpg", "flora_sonyalpha.jpg"}
        }.ToImmutableDictionary();

        /// <summary>
        ///     Initialize a new app context
        /// </summary>
        /// <param name="files">The files that should exist in the photo folder. Key should be the path (relative) and the value the file name of the resource</param>
        /// <returns></returns>
        public static async Task<ApplicationContext> Initialize(ITestOutputHelper output, IReadOnlyDictionary<string, string> files)
        {
            var app = ApplicationContext.Initialize(output);

            foreach (var file in files)
                app.AddResourceFile(Path.Combine(PhotoFolderPath, file.Key), file.Value);

            var creator = app.Container.Resolve<IPhotoDirectoryCreator>();
            await creator.Create(PhotoFolderPath, new PhotoDirectoryConfig("{date:yyyy}/{date:MM.dd} - {eventName}/{filename}"));

            var loader = app.Container.Resolve<IPhotoDirectoryLoader>();
            var photoDirectory = await loader.Load(PhotoFolderPath);

            using (var context = ((PhotoDirectory) photoDirectory).GetAppDbContext())
            {
                await context.Database.MigrateAsync();
            }

            var worker = app.Container.Resolve<ISynchronizeIndexWorker>();
            var response = await worker.Execute(new SynchronizeIndexRequest(photoDirectory));

            Assert.Collection(response.Operations.OrderBy(x => x.TargetFilename), files.OrderBy(x => x.Key).Select(x => new Action<FileOperation>(y =>
            {
                Assert.Equal(x.Key, y.TargetFilename);
                Assert.Equal(FileOperationType.New, y.Type);
            })).ToArray());

            return app;
        }
    }
}
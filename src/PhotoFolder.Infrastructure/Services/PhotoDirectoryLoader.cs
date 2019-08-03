using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Infrastructure.Consts;
using PhotoFolder.Infrastructure.Data;
using PhotoFolder.Infrastructure.Photos;
using PhotoFolder.Infrastructure.Serialization;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace PhotoFolder.Infrastructure.Services
{
    public interface IPhotoDirectoryLoader
    {
        Task<IPhotoDirectory> Load(string path);
    }

    public class PhotoDirectoryLoader : IPhotoDirectoryLoader
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDataSerializer _serializer;
        private readonly WorkspaceOptions _workspaceOptions;
        private readonly IAppDbContextOptionsBuilder _contextBuilder;

        public PhotoDirectoryLoader(IFileSystem fileSystem, IDataSerializer serializer,
            IAppDbContextOptionsBuilder contextBuilder, IOptions<WorkspaceOptions> workspaceOptions)
        {
            _fileSystem = fileSystem;
            _serializer = serializer;
            _workspaceOptions = workspaceOptions.Value;
            _contextBuilder = contextBuilder;
        }

        public async Task<IPhotoDirectory> Load(string path)
        {
            var configFilename = _fileSystem.Path.Combine(path, PhotoFolderConsts.ConfigFileName);
            var configFile = _fileSystem.FileInfo.FromFileName(configFilename);

            if (!configFile.Exists)
            {
                throw new FileNotFoundException("The photo directory config was not found.", configFilename);
            }

            var configContent = await _fileSystem.File.ReadAllTextAsync(configFilename);
            var config = _serializer.Deserialize<PhotoDirectoryConfig>(configContent);

            var workspacePath = _fileSystem.Path.Combine(Environment.ExpandEnvironmentVariables(_workspaceOptions.Path), config.Guid.ToString("D"));
            var databaseFile = _fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(workspacePath, "database.sqlite"));
            databaseFile.Directory.Create();

            var contextOptions = _contextBuilder.Build(databaseFile.FullName);
            using (var context = new AppDbContext(contextOptions))
            {
                await context.Database.MigrateAsync();
            }

            return new PhotoDirectory(_fileSystem, path, config, contextOptions);
        }
    }
}

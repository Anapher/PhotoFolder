using PhotoFolder.Infrastructure.Consts;
using PhotoFolder.Infrastructure.Photos;
using PhotoFolder.Infrastructure.Serialization;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace PhotoFolder.Infrastructure.Services
{
    public interface IPhotoDirectoryCreator
    {
        Task Create(string path, PhotoDirectoryConfig config);
    }

    public class PhotoDirectoryCreator : IPhotoDirectoryCreator
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDataSerializer _serializer;

        public PhotoDirectoryCreator(IFileSystem fileSystem, IDataSerializer serializer)
        {
            _fileSystem = fileSystem;
            _serializer = serializer;
        }

        public async Task Create(string path, PhotoDirectoryConfig config)
        {
            var configFilename = _fileSystem.Path.Combine(path, PhotoFolderConsts.ConfigFileName);

            var serializedConfig = _serializer.Serialize(config);
            await _fileSystem.File.WriteAllTextAsync(configFilename, serializedConfig);

            _fileSystem.File.SetAttributes(configFilename, FileAttributes.Hidden);
        }
    }
}

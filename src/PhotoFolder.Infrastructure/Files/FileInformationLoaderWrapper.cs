using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Services;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using IFile = PhotoFolder.Core.Dto.Services.IFile;

namespace PhotoFolder.Infrastructure.Files
{
    public class DiskLockedFileInformationLoader : IFileInformationLoader
    {
        private readonly FileInformationLoader _fileInformationLoader;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _driveLocks;
        private readonly IFileSystem _fileSystem;
        private readonly InfrastructureOptions _options;

        public DiskLockedFileInformationLoader(FileInformationLoader fileInformationLoader, IFileSystem fileSystem, IOptions<InfrastructureOptions> options)
        {
            _fileInformationLoader = fileInformationLoader;
            _driveLocks = new ConcurrentDictionary<string, SemaphoreSlim>();
            _fileSystem = fileSystem;
            _options = options.Value;
        }

        public async ValueTask<FileInformation> Load(IFile file)
        {
            var root = _fileSystem.Path.GetPathRoot(file.Filename);
            var volumeLock = _driveLocks.GetOrAdd(root, _ => new SemaphoreSlim(1, 1));
            await volumeLock.WaitAsync();

            if (file.Length > _options.LargeFileMargin)
                return await _fileInformationLoader.Load(file);

            Stream fileStream;
            try
            {
                fileStream = await _fileInformationLoader.LoadFileToMemory(file);
            }
            finally
            {
                volumeLock.Release();
            }

            await using (fileStream)
            {
                return _fileInformationLoader.Load(file, fileStream);
            }
        }
    }

    public class CachedFileInformationLoader : IFileInformationLoader
    {
        private readonly IFileInformationLoader _fileInformationLoader;
        private readonly ConcurrentDictionary<string, FileInformation> _cachedResults;

        public CachedFileInformationLoader(IFileInformationLoader fileInformationLoader)
        {
            _fileInformationLoader = fileInformationLoader;
            _cachedResults = new ConcurrentDictionary<string, FileInformation>();
        }

        public async ValueTask<FileInformation> Load(IFile file)
        {
            if (_cachedResults.TryGetValue(file.Filename, out var fileInformation))
                return fileInformation;

            var result = await _fileInformationLoader.Load(file);
            _cachedResults.TryAdd(file.Filename, result);

            return result;
        }
    }
}

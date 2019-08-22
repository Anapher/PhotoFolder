using System.IO.Abstractions;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Infrastructure.Utilities;

namespace PhotoFolder.Infrastructure.Files
{
    public class PathUtils : IPathUtils
    {
        private readonly IFileSystem _fileSystem;

        public PathUtils(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public char DirectorySeparator { get; } = '/';

        public string GetDirectoryName(string path) => _fileSystem.Path.GetDirectoryName(path).ToForwardSlashes();

        public string GetFileName(string path) => _fileSystem.Path.GetFileName(path).ToForwardSlashes();

        public string Combine(params string[] parts) => _fileSystem.Path.Combine(parts).ToForwardSlashes();
    }
}
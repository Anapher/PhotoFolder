using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Interfaces.Gateways;
using System.Collections.Immutable;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading.Tasks;

namespace PhotoFolder.Infrastructure.Photos
{
    public class DeletedFilesManager : IDeletedFiles
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _filename;

        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public DeletedFilesManager(IFileSystem fileSystem, string filename, IImmutableDictionary<string, DeletedFileInfo> deletedFiles)
        {
            _fileSystem = fileSystem;
            _filename = filename;

            Files = deletedFiles;
        }

        public IImmutableDictionary<string, DeletedFileInfo> Files { get; private set; }

        public static async Task<DeletedFilesManager> Load(IFileSystem fileSystem, string filename)
        {
            var exists = fileSystem.File.Exists(filename);
            if (!exists) return new DeletedFilesManager(fileSystem, filename, ImmutableDictionary<string, DeletedFileInfo>.Empty);

            var content = await fileSystem.File.ReadAllTextAsync(filename);
            var deletedFiles = JsonConvert.DeserializeObject<ImmutableDictionary<string, DeletedFileInfo>>(content, _jsonSerializerSettings);
            return new DeletedFilesManager(fileSystem, filename, deletedFiles);
        }

        public async Task Update(IImmutableDictionary<string, DeletedFileInfo> files)
        {
            var content = JsonConvert.SerializeObject(files, _jsonSerializerSettings);

            using (var fileStream = _fileSystem.FileStream.Create(_filename, FileMode.OpenOrCreate))
            {
                using (var textWriter = new StreamWriter(fileStream, Encoding.UTF8, 1024, true))
                    await textWriter.WriteAsync(content);

                fileStream.SetLength(fileStream.Position);
            }

            _fileSystem.File.SetAttributes(_filename, FileAttributes.Hidden);
            Files = files;
        }
    }
}

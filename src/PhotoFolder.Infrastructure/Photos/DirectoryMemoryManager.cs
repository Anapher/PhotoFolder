using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PhotoFolder.Core.Interfaces.Gateways;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading.Tasks;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Infrastructure.Json;

namespace PhotoFolder.Infrastructure.Photos
{
    public class DirectoryMemoryManager : IDirectoryMemoryManager
    {
        private readonly IFileSystem _fileSystem;
        private readonly string _filename;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented,
            Converters = new List<JsonConverter> {new HashConverter()}
        };

        public DirectoryMemoryManager(IFileSystem fileSystem, string filename, IDirectoryMemory directoryMemory)
        {
            _fileSystem = fileSystem;
            _filename = filename;

            DirectoryMemory = directoryMemory;
        }

        public static async Task<DirectoryMemoryManager> Load(IFileSystem fileSystem, string filename)
        {
            var exists = fileSystem.File.Exists(filename);
            if (!exists) return new DirectoryMemoryManager(fileSystem, filename, Photos.DirectoryMemory.Empty);

            var content = await fileSystem.File.ReadAllTextAsync(filename);
            var memory = JsonConvert.DeserializeObject<DirectoryMemory>(content, JsonSerializerSettings);
            return new DirectoryMemoryManager(fileSystem, filename, memory);
        }

        public IDirectoryMemory DirectoryMemory { get; private set; }

        public async ValueTask Update(IDirectoryMemory directoryMemory)
        {
            var content = JsonConvert.SerializeObject(directoryMemory, JsonSerializerSettings);

            await using (var fileStream = _fileSystem.FileStream.Create(_filename, FileMode.OpenOrCreate))
            {
                await using (var textWriter = new StreamWriter(fileStream, Encoding.UTF8, 1024, true))
                    await textWriter.WriteAsync(content);

                fileStream.SetLength(fileStream.Position);
            }

            _fileSystem.File.SetAttributes(_filename, FileAttributes.Hidden);
            DirectoryMemory = directoryMemory;
        }
    }
}

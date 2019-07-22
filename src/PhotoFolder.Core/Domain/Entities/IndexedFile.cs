using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Shared;

namespace PhotoFolder.Core.Domain.Entities
{
    public class IndexedFile : BaseEntity, IFileInfo
    {
        public IndexedFile(string filename, Hash fileHash, long length, PhotoProperties? photoProperties)
        {
            Filename = filename;
            FileHash = fileHash;
            Length = length;
            PhotoProperties = photoProperties;
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private IndexedFile()
        {
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        public string Filename { get; private set; }
        public Hash FileHash { get; private set; }
        public long Length { get; private set; }

        public PhotoProperties? PhotoProperties { get; private set; }
    }
}

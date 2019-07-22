using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Shared;
using System;

namespace PhotoFolder.Core.Domain.Entities
{
    public class IndexedFile : BaseEntity, IFileInfo
    {
        public IndexedFile(string filename, Hash hash, long length, DateTimeOffset creationDate)
        {
            Filename = filename;
            Hash = hash;
            Length = length;
            CreationDate = creationDate;
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private IndexedFile()
        {
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        public string Filename { get; }
        public Hash Hash { get; }
        public long Length { get; }
        public DateTimeOffset CreationDate { get; }
    }
}

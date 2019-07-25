using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoFolder.Core.Domain.Entities
{
    public class IndexedFile : IEntity, IFileContentInfo
    {
        private HashSet<FileLocation> _indexedFiles;

        public IndexedFile(Hash hash, long length, DateTimeOffset fileCreatedOn,
            PhotoProperties? photoProperties)
        {
            Hash = hash.ToString();
            Length = length;
            FileCreatedOn = fileCreatedOn;
            PhotoProperties = photoProperties;

            _indexedFiles = new HashSet<FileLocation>();
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private IndexedFile()
        {
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        public string Hash { get; private set; }
        public long Length { get; private set; }
        public DateTimeOffset FileCreatedOn { get; private set; }

        // this belongs to the entity
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        public PhotoProperties? PhotoProperties { get; private set; }
        public IEnumerable<FileLocation> Files => _indexedFiles;

        Hash IFileContentInfo.Hash => Core.Hash.Parse(Hash);

        public void AddLocation(FileLocation indexedFile)
        {
            if (Files.Any(x => x.Filename == indexedFile.Filename))
                throw new ArgumentException("The file is already added.", nameof(indexedFile.Filename));

            _indexedFiles.Add(indexedFile);
        }

        public FileLocation? GetFileByFilename(string filename)
        {
            return Files.FirstOrDefault(x => x.Filename == filename);
        }

        public void RemoveLocation(string filename)
        {
            var fileLocation = _indexedFiles.FirstOrDefault(x => x.Filename == filename);
            if (fileLocation != null)
                _indexedFiles.Remove(fileLocation);
            else
            {
                throw new ArgumentException("The file does not exist.", nameof(filename));
            }
        }
    }

}

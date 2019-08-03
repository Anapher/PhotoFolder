using System;

namespace PhotoFolder.Core.Domain.Entities
{
    public class FileLocation : IFileReference
    {
        public FileLocation(string filename, string fileHash, DateTimeOffset createdOn,
            DateTimeOffset modifiedOn)
        {
            Filename = filename;
            Hash = fileHash;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private FileLocation()
        {
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        public string Hash { get; protected set; }
        public string Filename { get; protected set; }

        // warning: these properties belong to the file, not the entity
        public DateTimeOffset CreatedOn { get; private set; }
        public DateTimeOffset ModifiedOn { get; private set; }

        public void ChangeFileHash(Hash hash)
        {
            Hash = hash.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoFolder.Core.Domain.Entities
{
    public class FileLocation : FileReference
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

        // warning: these properties belong to the file, not the entity
        public DateTimeOffset CreatedOn { get; }
        public DateTimeOffset ModifiedOn { get; }

        public void ChangeFileHash(Hash hash)
        {
            Hash = hash.ToString();
        }
    }
}

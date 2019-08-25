using System;

namespace PhotoFolder.Core.Domain.Entities
{
    public class FileLocation : IFileReference
    {
        public FileLocation(string filename, string fileHash, DateTimeOffset createdOn,
            DateTimeOffset modifiedOn)
        {
            RelativeFilename = filename;
            Hash = fileHash;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private FileLocation()
        {
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        /// <summary>
        ///     The file hash
        /// </summary>
        public string Hash { get; protected set; }

        /// <summary>
        ///     The relative path to the file
        /// </summary>
        public string RelativeFilename { get; protected set; }

        // warning: these properties belong to the file, not the entity
        /// <summary>
        ///     The creation date of the file
        /// </summary>
        public DateTimeOffset CreatedOn { get; private set; }

        /// <summary>
        ///     The modification date of the file
        /// </summary>
        public DateTimeOffset ModifiedOn { get; private set; }
    }
}

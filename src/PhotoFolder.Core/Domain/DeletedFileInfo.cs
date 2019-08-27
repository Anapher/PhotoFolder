using PhotoFolder.Core.Dto.Services;
using System;

namespace PhotoFolder.Core.Domain
{
    public class DeletedFileInfo : IFileContentInfo
    {
        public DeletedFileInfo(string relativeFilename, long length, Hash hash, PhotoProperties? photoProperties, DateTimeOffset fileCreatedOn, DateTimeOffset deletedAt)
        {
            RelativeFilename = relativeFilename;
            Length = length;
            Hash = hash;
            PhotoProperties = photoProperties;
            FileCreatedOn = fileCreatedOn;
            DeletedAt = deletedAt;
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private DeletedFileInfo()
        {
        }
#pragma warning restore CS8618

        public string RelativeFilename { get; private set; }

        public long Length { get; private set; }
        public Hash Hash { get; private set; }
        public PhotoProperties? PhotoProperties { get; private set; }

        public DateTimeOffset FileCreatedOn { get; private set; }
        public DateTimeOffset DeletedAt { get; private set; }
    }
}

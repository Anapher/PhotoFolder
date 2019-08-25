using System;
using PhotoFolder.Core.Domain;

namespace PhotoFolder.Core.Dto.Services
{
    public class FileInformation : IFileInfo, IFileContentInfo
    {
        public FileInformation(string filename, DateTimeOffset createdOn, DateTimeOffset modifiedOn, Hash hash,
                               long length, DateTimeOffset fileCreatedOn, PhotoProperties? photoProperties, string? relativeFilename)
        {
            Filename = filename;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
            Hash = hash;
            Length = length;
            FileCreatedOn = fileCreatedOn;
            PhotoProperties = photoProperties;
            RelativeFilename = relativeFilename;
        }

        public string Filename { get; }
        public string? RelativeFilename { get; set; }

        public DateTimeOffset CreatedOn { get; }
        public DateTimeOffset ModifiedOn { get; }

        public long Length { get; }
        public DateTimeOffset FileCreatedOn { get; }

        public PhotoProperties? PhotoProperties { get; }

        public Hash Hash { get; }
    }
}

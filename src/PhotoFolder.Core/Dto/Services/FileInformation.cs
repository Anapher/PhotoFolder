using PhotoFolder.Core.Dto.Services;
using System;

namespace PhotoFolder.Core.Domain.Entities
{
    public class FileInformation : FileReference, IFileInfo, IFileContentInfo
    {
        public FileInformation(string filename, DateTimeOffset createdOn, DateTimeOffset modifiedOn, string hash,
                               long length, DateTimeOffset fileCreatedOn, PhotoProperties? photoProperties)
        {
            Filename = filename;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
            Hash = hash.ToString();
            Length = length;
            FileCreatedOn = fileCreatedOn;
            PhotoProperties = photoProperties;
        }

        public DateTimeOffset CreatedOn { get; }
        public DateTimeOffset ModifiedOn { get; }

        public long Length { get; }
        public DateTimeOffset FileCreatedOn { get; }

        public PhotoProperties? PhotoProperties { get; }

        Hash IFileContentInfo.Hash => Core.Hash.Parse(Hash);
    }
}

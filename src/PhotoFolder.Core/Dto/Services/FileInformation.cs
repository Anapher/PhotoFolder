using System;
using PhotoFolder.Core.Domain;

namespace PhotoFolder.Core.Dto.Services
{
    public class FileInformation : FileReference, IFileInfo, IFileContentInfo
    {
        public FileInformation(string filename, DateTimeOffset createdOn, DateTimeOffset modifiedOn, Hash hash,
                               long length, DateTimeOffset fileCreatedOn, PhotoProperties? photoProperties, bool isRelativeFilename)
        {
            Filename = filename;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
            Hash = hash.ToString();
            Length = length;
            FileCreatedOn = fileCreatedOn;
            PhotoProperties = photoProperties;
            IsRelativeFilename = isRelativeFilename;
        }

        public DateTimeOffset CreatedOn { get; }
        public DateTimeOffset ModifiedOn { get; }

        public long Length { get; }
        public DateTimeOffset FileCreatedOn { get; }

        public PhotoProperties? PhotoProperties { get; }

        Hash IFileContentInfo.Hash => Domain.Hash.Parse(Hash);

        public bool IsRelativeFilename { get; }
    }
}

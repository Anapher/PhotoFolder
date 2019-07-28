using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoFolder.Core.Utilities
{
    public static class FileEntityTransformations
    {
        public static FileInformation ToFileInformation(this IndexedFile file, string filename)
        {
            var fileLocation = file.GetFileByFilename(filename);
            if (fileLocation == null)
                throw new ArgumentException($"The indexed file is not located at {filename}");

            return new FileInformation(
                filename, fileLocation.CreatedOn, fileLocation.ModifiedOn, Hash.Parse(file.Hash), file.Length, file.CreatedOn, file.PhotoProperties);
        }

        public static IEnumerable<IFileInfo> ToFileInfos(this IndexedFile file)
        {
            return file.Files.Select(x => new BasicFileInfo(x.Filename, file.Length, x.CreatedOn, x.ModifiedOn));
        }
    }

    public class BasicFileInfo : IFileInfo
    {
        public BasicFileInfo(string filename, long length, DateTimeOffset createdOn, DateTimeOffset modifiedOn)
        {
            Filename = filename;
            Length = length;
            CreatedOn = createdOn;
            ModifiedOn = modifiedOn;
        }

        public string Filename { get; }
        public long Length { get; }

        public DateTimeOffset CreatedOn { get; }
        public DateTimeOffset ModifiedOn { get; }
    }
}

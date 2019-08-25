using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Core.Interfaces.Gateways;

namespace PhotoFolder.Core.Utilities
{
    public static class FileEntityTransformations
    {
        public static FileInformation ToFileInformation(this IndexedFile file, string relativeFilename, IPhotoDirectory photoDirectory)
        {
            var fileLocation = file.GetFileByFilename(relativeFilename);
            if (fileLocation == null)
                throw new ArgumentException($"The indexed file is not located at {relativeFilename}");

            return photoDirectory.ToFileInformation(file, fileLocation);
        }

        public static FileInformation ToFileInformation(this IndexedFile file, IPhotoDirectory photoDirectory)
        {
            return photoDirectory.ToFileInformation(file, file.Files.First());
        }

        public static IEnumerable<IFileInfo> ToFileInfos(this IndexedFile file, IPhotoDirectory photoDirectory)
        {
            return file.Files.Select(x => photoDirectory.ToFileInformation(file, x));
        }
    }
}

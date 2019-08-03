using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PhotoFolder.Core;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoFolder.Infrastructure.Files
{
    public class FileInformationLoader : IFileInformationLoader
    {
        private readonly IFileHasher _fileHasher;
        private readonly IFileSystem _fileSystem;

        public FileInformationLoader(IFileHasher fileHasher, IFileSystem fileSystem)
        {
            _fileHasher = fileHasher;
            _fileSystem = fileSystem;
        }

        public async Task<FileInformation> Load(string filename)
        {
            var fileInfo = _fileSystem.FileInfo.FromFileName(filename);
            if (!fileInfo.Exists) throw new FileNotFoundException();

            var file = new FileInfoWrapper(fileInfo, "")
        }

        public async Task<FileInformation> Load(IFile file)
        {
            PhotoProperties? properties;
            Hash fileHash;
            DateTimeOffset fileCreatedOn;

            MemoryStream targetStream;
            using (var fileStream = file.OpenRead())
            {
                targetStream = new MemoryStream((int)fileStream.Length);
                await fileStream.CopyToAsync(targetStream);
            }

            using (targetStream)
            {
                targetStream.Position = 0;
                fileCreatedOn = GetCreationDate(targetStream, file);

                targetStream.Position = 0;
                try
                {
                    using (var bmp = new Bitmap(targetStream))
                    {
                        var width = bmp.Width;
                        var height = bmp.Height;
                        var bmpHash = new Hash(BitmapHash.Compute(bmp));

                        properties = new PhotoProperties(bmpHash, width, height);
                    }
                }
                catch (Exception)
                {
                    properties = null;
                }

                targetStream.Position = 0;
                fileHash = _fileHasher.ComputeHash(targetStream);
            }

            return new FileInformation(file.Filename, file.CreatedOn, file.ModifiedOn, fileHash, file.Length,
                                       fileCreatedOn, properties);
        }

        private static bool TrySelectDateTaken(IReadOnlyList<MetadataExtractor.Directory> metadata, out DateTime dateTime)
        {
            var exifDictionary = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (exifDictionary != null)
            {
                if (exifDictionary.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out dateTime))
                    return true;

                if (exifDictionary.TryGetDateTime(ExifDirectoryBase.TagDateTime, out dateTime))
                    return true;
            }

            dateTime = default;
            return false;
        }

        private static DateTimeOffset GetCreationDate(Stream stream, IFile file)
        {
            IReadOnlyList<MetadataExtractor.Directory> metadata;
            try
            {
                metadata = ImageMetadataReader.ReadMetadata(stream);
                if (TrySelectDateTaken(metadata, out var dateTime))
                    return dateTime;
            }
            catch (Exception)
            {
                // ignored
            }

            return file.ModifiedOn;
        }
    }
}

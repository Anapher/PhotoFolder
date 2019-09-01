using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PhotoFolder.Core.Dto.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PhotoFolder.Infrastructure.Files
{
    public class FileInformationLoader : IFileInformationLoader
    {
        private readonly IFileHasher _fileHasher;
        private readonly ILogger<FileInformationLoader> _logger;
        private readonly InfrastructureOptions _options;

        public FileInformationLoader(IFileHasher fileHasher, ILogger<FileInformationLoader> logger, IOptions<InfrastructureOptions> options)
        {
            _fileHasher = fileHasher;
            _logger = logger;
            _options = options.Value;
        }

        public async ValueTask<MemoryStream> LoadFileToMemory(IFile file)
        {
            MemoryStream targetStream;
            await using (var fileStream = file.OpenRead())
            {
                targetStream = new MemoryStream((int)fileStream.Length);
                await fileStream.CopyToAsync(targetStream);
            }

            targetStream.Position = 0;
            return targetStream;
        }

        public async ValueTask<FileInformation> Load(IFile file)
        {
            if (file.Length > _options.LargeFileMargin)
                return await LoadLargeFile(file);

            _logger.LogDebug("Load file {filename} into memory", file.Filename);

            await using var stream = await LoadFileToMemory(file);
            stream.Position = 0;
            return Load(file, stream);
        }

        private async ValueTask<FileInformation> LoadLargeFile(IFile file)
        {
            await using var fileStream = file.OpenRead();
            var hash = _fileHasher.ComputeHash(fileStream);

            return new FileInformation(file.Filename, file.CreatedOn, file.ModifiedOn, hash, file.Length, file.CreatedOn, null, file.RelativeFilename);
        }

        public FileInformation Load(IFile file, Stream stream)
        {
            _logger.LogDebug("Load file information of {filename}", file.Filename);

            PhotoProperties? properties;
            Hash fileHash;
            DateTimeOffset fileCreatedOn;

            fileCreatedOn = GetCreationDate(stream, file);

            stream.Position = 0;
            try
            {
                using var bmp = new Bitmap(stream);

                var width = bmp.Width;
                var height = bmp.Height;
                var bmpHash = new Hash(BitmapHash.Compute(bmp));

                properties = new PhotoProperties(bmpHash, width, height);
            }
            catch (Exception)
            {
                properties = null;
            }

            stream.Position = 0;
            fileHash = _fileHasher.ComputeHash(stream);

            var fileInformation =  new FileInformation(file.Filename, file.CreatedOn, file.ModifiedOn, fileHash, file.Length,
                           fileCreatedOn, properties, file.RelativeFilename);
            _logger.LogDebug("FileInformation of file {filename} loaded: {@data}", file.Filename, fileInformation);

            return fileInformation;
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

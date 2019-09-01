using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoFolder.Core.Domain.Entities
{
    public class IndexedFile : IEntity, IFileContentInfo
    {
        private HashSet<FileLocation> _files;
        private int? _photoPropertiesWidth;
        private int? _photoPropertiesHeight;
        private string? _photoPropertiesBitmapHash;
        private readonly Lazy<PhotoProperties?> _lazyPhotoProperties;

        public IndexedFile(Hash hash, long length, DateTimeOffset fileCreatedOn,
            PhotoProperties? photoProperties) : this()
        {
            Hash = hash.ToString();
            Length = length;
            FileCreatedOn = fileCreatedOn;

            if (photoProperties != null)
            {
                _photoPropertiesWidth = photoProperties.Width;
                _photoPropertiesHeight = photoProperties.Height;
                _photoPropertiesBitmapHash = photoProperties.BitmapHash.ToString();
            }

            _files = new HashSet<FileLocation>();
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized.
        private IndexedFile()
        {
            _lazyPhotoProperties = new Lazy<PhotoProperties?>(() =>
            {
                if (_photoPropertiesBitmapHash != null && _photoPropertiesWidth != null && _photoPropertiesHeight != null)
                    return new PhotoProperties(Domain.Hash.Parse(_photoPropertiesBitmapHash), _photoPropertiesWidth.Value, _photoPropertiesHeight.Value);

                return null;
            });
        }
#pragma warning restore CS8618 // Non-nullable field is uninitialized.

        public string Hash { get; private set; }
        public long Length { get; private set; }
        public DateTimeOffset FileCreatedOn { get; private set; }

        // this belongs to the entity
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ModifiedOn { get; set; }

        public PhotoProperties? PhotoProperties => _lazyPhotoProperties.Value;
        public IEnumerable<FileLocation> Files => _files;

        Hash IFileContentInfo.Hash => Domain.Hash.Parse(Hash);

        public void AddLocation(FileLocation location)
        {
            if (Files.Any(x => x.RelativeFilename == location.RelativeFilename))
                throw new ArgumentException("The file is already added.", nameof(location.RelativeFilename));

            _files.Add(location);
        }

        public FileLocation? GetFileByFilename(string filename)
        {
            return Files.FirstOrDefault(x => x.RelativeFilename == filename);
        }

        public bool HasPath(string filename) => Files.Any(x => x.RelativeFilename == filename);

        public void RemoveLocation(string filename)
        {
            var fileLocation = _files.FirstOrDefault(x => x.RelativeFilename == filename);
            if (fileLocation != null)
                _files.Remove(fileLocation);
            else
            {
                throw new ArgumentException("The file does not exist.", nameof(filename));
            }
        }
    }

}

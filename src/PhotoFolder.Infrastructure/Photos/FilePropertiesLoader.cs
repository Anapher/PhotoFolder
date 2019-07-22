using MetadataExtractor;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Services;
using System;
using System.Drawing;
using System.Threading.Tasks;

namespace PhotoFolder.Infrastructure.Photos
{
    public class FilePropertiesLoader : IFilePropertiesLoader
    {
        public Task<FileProperties> Load(IFile file)
        {
            var fileStream = file.OpenRead();
            try
            {
                var metadata = ImageMetadataReader.ReadMetadata(fileStream);

                if (fileStream.CanSeek)
                    fileStream.Position = 0;
                else
                {
                    fileStream.Dispose();
                    fileStream = file.OpenRead();
                }

                var bmp = new Bitmap(fileStream);
                // https://github.com/drewnoakes/metadata-extractor-dotnet
                // https://stackoverflow.com/questions/35151067/algorithm-to-compare-two-images-in-c-sharp
                throw new NotImplementedException();
            }
            finally
            {
                fileStream?.Dispose();
            }
        }
    }
}

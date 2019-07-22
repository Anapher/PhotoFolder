using MetadataExtractor;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Infrastructure.Utilities;
using System;
using System.Drawing;
using System.Security.Cryptography;
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
                using (var sha256 = SHA256.Create())
                using (var cryptoStream = new CryptoStream(fileStream, sha256, CryptoStreamMode.Read, true))
                {
                    try
                    {
                        using (var bmp = new Bitmap(cryptoStream))
                        {
                            var bitmapHash = BitmapHash.Compute(bmp);
                            var props = new PhotoProperties(bitmapHash, )
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                // https://github.com/drewnoakes/metadata-extractor-dotnet
                // https://stackoverflow.com/questions/35151067/algorithm-to-compare-two-images-in-c-sharp
                throw new NotImplementedException();

                //var metadata = ImageMetadataReader.ReadMetadata(fileStream);

                //if (fileStream.CanSeek)
                //    fileStream.Position = 0;
                //else
                //{
                //    fileStream.Dispose();
                //    fileStream = file.OpenRead();
                //}
            }
            finally
            {
                fileStream?.Dispose();
            }
        }

        private static DateTimeOffset? GetDateTaken()
        {

        }
    }
}

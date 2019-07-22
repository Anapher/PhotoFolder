using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Dto.Services;
using System;
using System.Threading.Tasks;

namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IFilePropertiesLoader
    {
        Task<FileProperties> Load(IFile file);
    }

    public class FileProperties
    {
        public FileProperties(Hash fileHash, PhotoProperties photoProperties)
        {
            FileHash = fileHash;
            PhotoProperties = photoProperties;
        }

        public Hash FileHash { get; }
        public PhotoProperties PhotoProperties { get; }
    }
}

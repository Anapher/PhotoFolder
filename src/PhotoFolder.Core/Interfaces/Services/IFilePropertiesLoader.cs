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
        public Hash Hash { get; }
        public Hash BitmapHash { get; }
        public DateTimeOffset DateTaken { get; }
    }
}

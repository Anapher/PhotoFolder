using System.IO;

namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IFileHasher
    {
        Hash ComputeHash(Stream stream);
    }
}

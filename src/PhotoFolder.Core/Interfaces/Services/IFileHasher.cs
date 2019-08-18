using System.IO;
using PhotoFolder.Core.Domain;

namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IFileHasher
    {
        Hash ComputeHash(Stream stream);
    }
}

using PhotoFolder.Core;
using PhotoFolder.Core.Interfaces.Services;
using System.IO;
using System.Security.Cryptography;

namespace PhotoFolder.Infrastructure.Files
{
    public class SHA256FileHasher : IFileHasher
    {
        public Hash ComputeHash(Stream stream)
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(stream);
                return new Hash(hash);
            }
        }
    }
}

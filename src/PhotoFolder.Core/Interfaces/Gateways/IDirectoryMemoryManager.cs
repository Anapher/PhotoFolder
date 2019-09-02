using System.Threading.Tasks;
using PhotoFolder.Core.Interfaces.Services;

namespace PhotoFolder.Core.Interfaces.Gateways
{
    public interface IDirectoryMemoryManager
    {
        IDirectoryMemory DirectoryMemory { get; }
        ValueTask Update(IDirectoryMemory directoryMemory);
    }
}

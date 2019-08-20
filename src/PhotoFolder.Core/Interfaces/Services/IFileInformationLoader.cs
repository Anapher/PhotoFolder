using PhotoFolder.Core.Dto.Services;
using System.Threading.Tasks;

namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IFileInformationLoader
    {
        Task<FileInformation> Load(IFile file);
    }
}

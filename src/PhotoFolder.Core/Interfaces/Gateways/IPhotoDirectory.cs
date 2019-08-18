using PhotoFolder.Core.Dto.Services;
using System.Collections.Generic;

namespace PhotoFolder.Core.Interfaces.Gateways
{
    public interface IPhotoDirectory
    {
        IEqualityComparer<string> PathComparer { get; }

        IEnumerable<IFile> EnumerateFiles();
        IFile? GetFile(string filename);
        bool IsFileInDirectory(string filename);

        string GetFileDirectoryRegexPattern(FileInformation fileInformation);
        string GetFilenameRegexPattern(FileInformation fileInformation);

        string GetRecommendedPath(FileInformation fileInformation);

        IPhotoDirectoryDataContext GetDataContext();
    }
}

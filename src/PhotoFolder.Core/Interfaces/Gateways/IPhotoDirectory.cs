using PhotoFolder.Core.Dto.Services;
using System.Collections.Generic;
using PhotoFolder.Core.Domain.Template;

namespace PhotoFolder.Core.Interfaces.Gateways
{
    public interface IPhotoDirectory
    {
        IEqualityComparer<string> PathComparer { get; }

        IEnumerable<IFile> EnumerateFiles();
        IFile? GetFile(string filename);
        string GetAbsolutePath(FileInformation fileInformation);
        string ClearPath(string path);

        TemplateString GetFileDirectoryTemplate(FileInformation fileInformation);
        TemplateString GetFilenameTemplate(FileInformation fileInformation);

        string GetRecommendedPath(FileInformation fileInformation);

        IPhotoDirectoryDataContext GetDataContext();
    }
}

using PhotoFolder.Core.Dto.Services;
using System.Collections.Generic;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Domain.Template;

namespace PhotoFolder.Core.Interfaces.Gateways
{
    public interface IPhotoDirectory
    {
        string RootDirectory { get; }

        IEqualityComparer<string> PathComparer { get; }
        IDirectoryMemoryManager MemoryManager { get; }

        IEnumerable<IFile> EnumerateFiles();
        IFile? GetFile(string filename);
        string ClearPath(string path);

        TemplateString GetFileDirectoryTemplate(FileInformation fileInformation);
        TemplateString GetFilenameTemplate(FileInformation fileInformation);

        string GetRecommendedPath(FileInformation fileInformation);

        FileInformation ToFileInformation(IndexedFile indexedFile, FileLocation fileLocation);

        IPhotoDirectoryDataContext GetDataContext();
    }
}

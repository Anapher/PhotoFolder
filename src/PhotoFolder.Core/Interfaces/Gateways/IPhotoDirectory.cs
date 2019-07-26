using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Interfaces.Gateways.Repositories;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PhotoFolder.Core.Interfaces.Gateways
{
    public interface IPhotoDirectory
    {
        IEqualityComparer<string> PathComparer { get; }

        IEnumerable<IFile> EnumerateFiles();
        IFile? GetFile(string filename);

        string GetFilePathRegexPattern(FileInformation fileInformation);
        string GetRecommendedPath(FileInformation fileInformation);

        IIndexedFileRepository GetFileRepository();
        IFileOperationRepository GetOperationRepository();
    }
}

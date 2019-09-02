using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoFolder.Core.Services.FileIntegrityValidators
{
    public class SimilarFileIntegrityValidator : IFileIntegrityValidator
    {
        public ValueTask<IEnumerable<IFileIssue>> CheckForIssues(FileInformation file, IFileBaseContext fileBaseContext, IPhotoDirectoryDataContext dataContext)
        {
            return new ValueTask<IEnumerable<IFileIssue>>(CheckForIssues(file, fileBaseContext));
        }

        private IEnumerable<IFileIssue> CheckForIssues(FileInformation file, IFileBaseContext fileBaseContext)
        {
            if (file.PhotoProperties != null)
            {
                var similarFiles = new List<SimilarFile>();
                var fileHash = file.Hash.ToString();

                foreach (var (indexedFile, similarity) in fileBaseContext.FindSimilarFiles(file.PhotoProperties.BitmapHash))
                {
                    if (indexedFile.Hash == fileHash) continue; // can't be similar if it's equal

                    similarFiles.Add(new SimilarFile(indexedFile.ToFileInformation(fileBaseContext.PhotoDirectory), similarity));
                }

                if (similarFiles.Any())
                    yield return new SimilarFilesIssue(file, similarFiles);
            }
        }
    }
}

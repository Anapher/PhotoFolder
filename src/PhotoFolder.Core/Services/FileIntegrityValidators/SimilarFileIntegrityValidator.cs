using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Core.Domain;

namespace PhotoFolder.Core.Services.FileIntegrityValidators
{
    public class SimilarFileIntegrityValidator : IFileIntegrityValidator
    {
        private const float SimilarityThreshold = 0.8f;

        private readonly IBitmapHashComparer _bitmapHashComparer;

        public SimilarFileIntegrityValidator(IBitmapHashComparer bitmapHashComparer)
        {
            _bitmapHashComparer = bitmapHashComparer;
        }

        public IEnumerable<IFileIssue> CheckForIssues(FileInformation file, IPhotoDirectory photoDirectory, IReadOnlyList<IndexedFile> indexedFiles)
        {
            if (file.PhotoProperties != null)
            {
                var similarFiles = new List<SimilarFile>();

                foreach (var indexedFile in indexedFiles)
                {
                    if (indexedFile.PhotoProperties == null) continue;
                    if (Hash.Parse(indexedFile.Hash).Equals(file.Hash)) continue; // can't be similar if it's equal

                    var result = _bitmapHashComparer.Compare(indexedFile.PhotoProperties.BitmapHash,
                        file.PhotoProperties.BitmapHash);

                    if (result > SimilarityThreshold)
                        similarFiles.Add(new SimilarFile(indexedFile.ToFileInformation(photoDirectory), result));
                }

                if (similarFiles.Any())
                    yield return new SimilarFilesIssue(file, similarFiles);
            }
        }
    }
}

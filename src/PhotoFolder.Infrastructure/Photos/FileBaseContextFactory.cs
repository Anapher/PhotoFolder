using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Services;
using PhotoFolder.Core.Specifications.FileInformation;

namespace PhotoFolder.Infrastructure.Photos
{
    public class FileBaseContextFactory : IFileBaseContextFactory
    {
        private readonly ISimilarityDictionaryFactory _similarityDictionaryFactory;

        public FileBaseContextFactory(ISimilarityDictionaryFactory similarityDictionaryFactory)
        {
            _similarityDictionaryFactory = similarityDictionaryFactory;
        }

        public async ValueTask<IFileBaseContext> BuildContext(IPhotoDirectory photoDirectory)
        {
            IReadOnlyList<IndexedFile> indexedFiles;
            await using (var context = photoDirectory.GetDataContext())
            {
                indexedFiles = await context.FileRepository.GetAllReadOnlyBySpecs(new IncludeFileLocationsSpec());
            }

            return await BuildContext(photoDirectory, indexedFiles);
        }

        public ValueTask<IFileBaseContext> BuildContext(IPhotoDirectory photoDirectory, IReadOnlyList<IndexedFile> indexedFiles)
        {
            var dictionary = indexedFiles.ToDictionary(x => x.Hash, x => x);
            var similarityDictionary = _similarityDictionaryFactory.Create(indexedFiles);
            return new ValueTask<IFileBaseContext>(new FileBaseContext(photoDirectory, indexedFiles, dictionary, similarityDictionary));
        }
    }
}

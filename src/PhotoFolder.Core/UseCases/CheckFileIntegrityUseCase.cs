#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Interfaces.UseCases;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhotoFolder.Core.UseCases
{
    public class CheckFileIntegrityUseCase : UseCaseStatus<CheckFileIntegrityResponse>, ICheckFileIntegrityUseCase
    {
        private const float SimilarityThreshold = 0.8f;

        private readonly IBitmapHashComparer _bitmapHashComparer;

        public CheckFileIntegrityUseCase(IBitmapHashComparer bitmapHashComparer)
        {
            _bitmapHashComparer = bitmapHashComparer;
        }

        public async Task<CheckFileIntegrityResponse?> Handle(CheckFileIntegrityRequest message)
        {
            var directory = message.PhotoDirectory;
            var fileInformation = message.FileInformation;

            var similarFiles = new List<SimilarFile>();
            var equalFiles = new List<FileLocation>();
            var isWrongPlaced = false;
            IReadOnlyList<string>? recommendedDirectories = null;
            string? recommendedFilename = null;

            foreach (var indexedFile in message.IndexedFiles)
            {
                if (indexedFile.Hash == message.FileInformation.Hash)
                {
                    equalFiles = indexedFile.Files.Where(x => !directory.PathComparer.Equals(x.Filename, fileInformation.Filename)).ToList();
                    continue;
                }

                if (fileInformation.PhotoProperties == null || indexedFile.PhotoProperties == null)
                    continue;

                var result = _bitmapHashComparer.Compare(indexedFile.PhotoProperties.BitmapHash,
                    fileInformation.PhotoProperties.BitmapHash);

                if (result > SimilarityThreshold)
                    similarFiles.Add(new SimilarFile(indexedFile, result));
            }

            var pathPattern = directory.GetFileDirectoryRegexPattern(fileInformation);
            var pathRegex = new Regex(pathPattern);

            if (!pathRegex.IsMatch(Path.GetDirectoryName(message.FileInformation.Filename)))
            {
                isWrongPlaced = true;

                // query all files that have a directory that would match
                recommendedDirectories = message.IndexedFiles.SelectMany(x => x.Files).Select(x => Path.GetDirectoryName(x.Filename))
                    .Where(x => pathRegex.IsMatch(x)).Distinct().ToList();
            }

            var filenamePattern = directory.GetFilenameRegexPattern(fileInformation);
            if (!Regex.IsMatch(Path.GetFileName(message.FileInformation.Filename), filenamePattern))
            {
                recommendedFilename = Path.GetFileName(directory.GetRecommendedPath(fileInformation));
                isWrongPlaced = true;
            }

            return new CheckFileIntegrityResponse(equalFiles, similarFiles, isWrongPlaced,
                recommendedDirectories, recommendedFilename);
        }
    }
}

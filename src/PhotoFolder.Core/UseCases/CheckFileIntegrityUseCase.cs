using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto;
using PhotoFolder.Core.Dto.UseCaseRequests;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces;
using PhotoFolder.Core.Interfaces.Services;
using PhotoFolder.Core.Interfaces.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoFolder.Core.UseCases
{
    public class CheckFileIntegrityUseCase : UseCaseStatus<CheckFileIntegrityResponse>, ICheckFileIntegrityUseCase
    {
        private const float SimliarityThreshhold = 0.8f;

        private readonly IBitmapHashComparer _bitmapHashComparer;

        public CheckFileIntegrityUseCase(IBitmapHashComparer bitmapHashComparer)
        {
            _bitmapHashComparer = bitmapHashComparer;
        }

        public async Task<CheckFileIntegrityResponse?> Handle(CheckFileIntegrityRequest message)
        {
            var directory = message.PhotoDirectory;
            var fileInformation = message.FileInformation;

            var simliarFiles = new Dictionary<IndexedFile, float>();
            var equalFiles = new List<FileLocation>();

                foreach (var indexedFile in message.IndexedFiles)
                {
                    if (indexedFile.Hash == message.FileInformation.Hash)
                    {
                        equalFiles = indexedFile.Files.Where(x => directory.PathComparer.Equals(x.Filename, fileInformation.Filename)).ToList();
                        continue;
                    }

                if (fileInformation.PhotoProperties == null)
                    continue;

                    if (indexedFile.PhotoProperties == null)
                        continue;

                    var result = _bitmapHashComparer.Compare(indexedFile.PhotoProperties.BitmapHash,
                        fileInformation.PhotoProperties.BitmapHash);

                    if (result > SimliarityThreshhold)
                        simliarFiles.Add(indexedFile, result);
                }

            var pathMatcher = directory.GetFilePathRegexPattern(fileInformation);

            return null;
        }
    }
}

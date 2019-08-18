using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.Extensions;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Services
{
    public static class ImportDecisionFactory
    {
        public static List<IFileDecisionViewModel> Create(FileCheckReport report, IPhotoDirectory photoDirectory)
        {
            var result = new List<IFileDecisionViewModel>();
            foreach (var fileGroup in report.ProblematicFiles.GroupBy(x => x.Item1.Hash))
            {
                var integrityResponse = fileGroup.First().Item2;
                result.AddRange(Create(fileGroup.Select(x => x.Item1).ToList(), integrityResponse, photoDirectory));
            }

            return result;
        }

        public static IEnumerable<IFileDecisionViewModel> Create(IReadOnlyList<FileInformation> files,
            CheckFileIntegrityResponse report, IPhotoDirectory photoDirectory)
        {
            var fileLocations = files.GroupBy(x => photoDirectory.IsFileInDirectory(x.Filename)).ToList();
            bool isInDirectory;

            if (fileLocations.Count == 1)
                isInDirectory = fileLocations.Single().Key;
            else
                // we received equal files, some of them that were in the photo directory and some that weren't
                // we split these files and do a recursive call
                return fileLocations.SelectMany(x => Create(x.ToList(), report, photoDirectory));

            var analysis = files.Select(x => Create(x, report, photoDirectory, isInDirectory));
            if (files.Count == 1)
                return analysis.Single().Yield();

            return new EqualFileDecisionViewModel(analysis.ToList()).Yield();
        }

        public static IFileDecisionViewModel Create(FileInformation file, CheckFileIntegrityResponse report,
            IPhotoDirectory photoDirectory, bool isInPhotoDirectory)
        {
            var analysis = new List<IFileAnalysis>();
            if (report.EqualFiles.Any()) analysis.Add(new FileDuplicationAnalysis(report.EqualFiles));

            if (report.SimilarFiles.Any()) analysis.Add(new SimilarFilesAnalysis(report.SimilarFiles));

            if (report.IsWrongPlaced)
            {
                var filename = report.RecommendedFilename ?? Path.GetFileName(file.Filename);

                IReadOnlyList<string> directories;
                if (report.RecommendedDirectories != null)
                {
                    directories = report.RecommendedDirectories
                        .Concat(new[] {Path.GetDirectoryName(photoDirectory.GetRecommendedPath(file))}).Distinct().ToList();
                }
                else
                {
                    directories = new[] {Path.GetDirectoryName(file.Filename)};
                }

                analysis.Add(new WrongPlacedFileAnalysis(directories.Select(x => Path.Combine(x, filename))
                    .ToImmutableList()));
            }

            var pathTemplate = photoDirectory.GetFileDirectoryRegexPattern(file);
            return new FileDecisionViewModel(file, pathTemplate, analysis, isInPhotoDirectory);
        }
    }
}
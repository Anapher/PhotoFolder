using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.Services
{
    public static class ImportDecisionFactory
    {
        public static IEnumerable<IIssueDecisionViewModel> Create(FileCheckReport report, IPhotoDirectory photoDirectory)
        {
            foreach (var issue in report.Issues)
                if (issue is DuplicateFilesIssue duplicateFilesIssue)
                    yield return Create(duplicateFilesIssue, photoDirectory);
                else if (issue is InvalidFileLocationIssue invalidFileLocationIssue)
                    yield return new InvalidLocationFileDecisionViewModel(invalidFileLocationIssue);
                else if (issue is SimilarFilesIssue similarFilesIssue)
                    yield return new SimilarFileDecisionViewModel(similarFilesIssue,
                        similarFilesIssue.RelevantFiles.Concat(new[] {similarFilesIssue.File}).Select(x => new Checkable<FileInformation>(x, true)).ToList());
        }

        private static IIssueDecisionViewModel Create(DuplicateFilesIssue duplicateFilesIssue, IPhotoDirectory photoDirectory)
        {
            IEnumerable<Checkable<FileInformation>> filesToKeep;
            if (duplicateFilesIssue.File.RelativeFilename != null)
            {
                // just keep one file, try to find the file that is located correctly
                var bestFile = duplicateFilesIssue.RelevantFiles.FirstOrDefault(x =>
                                   x.RelativeFilename != null && Regex.IsMatch(x.RelativeFilename, photoDirectory.GetFilenameTemplate(x).ToRegexPattern())) ??
                               duplicateFilesIssue.File;

                filesToKeep = new[] {duplicateFilesIssue.File}.Concat(duplicateFilesIssue.RelevantFiles)
                    .Select(x => new Checkable<FileInformation>(x, x == bestFile));
            }
            else
            {
                // in case of import, we just delete the file that would be imported
                filesToKeep = new[] {new Checkable<FileInformation>(duplicateFilesIssue.File)}.Concat(
                    duplicateFilesIssue.RelevantFiles.Select(x => new Checkable<FileInformation>(x, true)));
            }

            return new DuplicateFileDecisionViewModel(duplicateFilesIssue, filesToKeep.ToList());
        }
    }
}
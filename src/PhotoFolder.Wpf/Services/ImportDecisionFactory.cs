using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PhotoFolder.Application.Dto.WorkerResponses;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Wpf.ViewModels;

namespace PhotoFolder.Wpf.Services
{
    public static class ImportDecisionFactory
    {
        public static IEnumerable<IIssueDecisionViewModel> Create(FileCheckReport report, IPhotoDirectory photoDirectory)
        {
            foreach (var issue in report.Issues)
            {
                if (issue is DuplicateFilesIssue duplicateFilesIssue)
                    yield return Create(duplicateFilesIssue, photoDirectory);
                else if (issue is InvalidFileLocationIssue invalidFileLocationIssue)
                    yield return new InvalidLocationFileDecisionViewModel(invalidFileLocationIssue);
                else if (issue is SimilarFileDecisionViewModel similarFileDecisionViewModel)
                    yield return new SimilarFileDecisionViewModel()
            }
        }

        public static IIssueDecisionViewModel Create(DuplicateFilesIssue duplicateFilesIssue, IPhotoDirectory photoDirectory)
        {
            var isInDirectory = duplicateFilesIssue.File.IsRelativeFilename;

            IEnumerable<Checkable<FileInformation>> filesToKeep;
            if (isInDirectory)
            {
                // just keep one file, try to find the file that is located correctly
                var bestFile = duplicateFilesIssue.RelevantFiles.FirstOrDefault(x => Regex.IsMatch(x.Filename, photoDirectory.GetFilenameRegexPattern(x))) ?? duplicateFilesIssue.File;
                filesToKeep = new[] { duplicateFilesIssue.File }.Concat(duplicateFilesIssue.RelevantFiles).Select(x => new Checkable<FileInformation>(x, x == bestFile));
            }
            else
            {
                // in case of import, we just delete the file that would be imported
                filesToKeep = new[] { new Checkable<FileInformation>(duplicateFilesIssue.File) }.Concat(duplicateFilesIssue.RelevantFiles.Select(x => new Checkable<FileInformation>(x, true)));
            }

            return new DuplicateFileDecisionViewModel(filesToKeep.ToList(), duplicateFilesIssue);
        }
    }
}
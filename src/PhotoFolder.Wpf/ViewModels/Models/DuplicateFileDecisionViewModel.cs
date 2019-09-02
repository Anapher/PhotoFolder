using System.Collections.Generic;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;

namespace PhotoFolder.Wpf.ViewModels.Models
{
    public class DuplicateFileDecisionViewModel : DeleteFilesDecisionViewModel
    {
        public DuplicateFileDecisionViewModel(DuplicateFilesIssue fileIssue, IReadOnlyList<Checkable<FileInformation>> files) : base(fileIssue, files)
        {
            Issue = fileIssue;
        }

        public override bool IsRecommended { get; } = true;
        public DuplicateFilesIssue Issue { get; }
    }
}
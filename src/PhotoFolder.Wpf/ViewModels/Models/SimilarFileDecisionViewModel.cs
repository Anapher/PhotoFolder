using System.Collections.Generic;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;

namespace PhotoFolder.Wpf.ViewModels.Models
{
    public class SimilarFileDecisionViewModel : DeleteFilesDecisionViewModel
    {
        public SimilarFileDecisionViewModel(SimilarFilesIssue fileIssue, IReadOnlyList<Checkable<FileInformation>> files) : base(fileIssue, files)
        {
            Issue = fileIssue;
        }

        public override bool IsRecommended { get; } = false;
        public SimilarFilesIssue Issue { get; }
    }
}
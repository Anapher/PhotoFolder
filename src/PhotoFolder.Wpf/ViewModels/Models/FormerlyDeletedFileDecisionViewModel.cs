using System.Linq;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Extensions;

namespace PhotoFolder.Wpf.ViewModels.Models
{
    public class FormerlyDeletedFileDecisionViewModel : DeleteFilesDecisionViewModel
    {
        public FormerlyDeletedFileDecisionViewModel(FormerlyDeletedIssue formerlyDeletedIssue) : base(formerlyDeletedIssue, formerlyDeletedIssue.File.Yield().Select(x => new Checkable<FileInformation>(x)).ToList())
        {
            Issue = formerlyDeletedIssue;
        }

        public override bool IsRecommended { get; } = true;
        public FormerlyDeletedIssue Issue { get; }
    }
}
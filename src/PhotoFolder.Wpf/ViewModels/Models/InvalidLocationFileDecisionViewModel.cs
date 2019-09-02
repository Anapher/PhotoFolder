using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Application.Dto;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels.Models
{
    public class InvalidLocationFileDecisionViewModel : BindableBase, IIssueDecisionViewModel
    {
        private string _targetPath;
        private IReadOnlyList<IFileOperation> _operations;

        public InvalidLocationFileDecisionViewModel(InvalidFileLocationIssue issue)
        {
            Issue = issue;

            _targetPath = issue.Suggestions.First().Filename;
            _operations = GetOperations();
        }

        public InvalidFileLocationIssue Issue { get; }
        IFileIssue IIssueDecisionViewModel.Issue => Issue;

        public bool IsRecommended { get; } = true;

        public IReadOnlyList<IFileOperation> Operations
        {
            get => _operations;
            private set => SetProperty(ref _operations, value);
        }

        public string TargetPath
        {
            get => _targetPath;
            set
            {
                if (SetProperty(ref _targetPath, value))
                    Operations = GetOperations();
            }
        }

        private IReadOnlyList<IFileOperation> GetOperations() => new[] {new MoveFileOperation(Issue.File, TargetPath)};

        public bool UpdateDeletedFiles(IReadOnlyList<FileInformation> deletedFiles)
        {
            return deletedFiles.All(x => x.Filename != Issue.File.Filename);
        }
    }
}
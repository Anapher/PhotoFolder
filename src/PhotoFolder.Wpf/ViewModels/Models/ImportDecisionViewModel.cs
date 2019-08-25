using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels.Models
{
    public interface IFileOperation
    {
        FileInformation File { get; }
    }

    public class DeleteFileOperation : IFileOperation
    {
        public DeleteFileOperation(FileInformation file)
        {
            File = file;
        }

        public FileInformation File { get; }
    }

    public class MoveFileOperation : IFileOperation
    {
        public MoveFileOperation(FileInformation file, string targetPath)
        {
            File = file;
            TargetPath = targetPath;
        }

        public FileInformation File { get; }
        public string TargetPath { get; }
    }

    public interface IIssueDecisionViewModel : INotifyPropertyChanged
    {
        bool IsRecommended { get; }
        IReadOnlyList<IFileOperation> Operations { get; }
        IFileIssue Issue { get; }

        bool UpdateDeletedFiles(IReadOnlyList<FileInformation> deletedFiles);
    }

    public class IssueDecisionWrapperViewModel : BindableBase
    {
        private bool _execute;
        private bool _isVisible;

        public IssueDecisionWrapperViewModel(IIssueDecisionViewModel decision)
        {
            Decision = decision;
            IsDeleteDecision = decision is DeleteFilesDecisionViewModel;
        }

        public IIssueDecisionViewModel Decision { get; }
        public bool IsDeleteDecision { get; }

        public bool Execute
        {
            get => _execute;
            set => SetProperty(ref _execute, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            private set=> SetProperty(ref _isVisible, value);
        }

        public void UpdateDeletedFiles(IReadOnlyList<FileInformation> deletedFiles)
        {
            IsVisible = Decision.UpdateDeletedFiles(deletedFiles);
        }
    }

    public class Checkable<T> : BindableBase
    {
        private bool _isChecked;

        public Checkable(T value, bool isChecked)
        {
            Value = value;
            IsChecked = isChecked;
        }

        public Checkable(T value) : this(value, false)
        {
        }

        public T Value { get; }

        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }
    }

    public abstract class DeleteFilesDecisionViewModel : BindableBase, IIssueDecisionViewModel
    {
        protected readonly IReadOnlyList<Checkable<FileInformation>> _allFiles;
        protected IReadOnlyList<Checkable<FileInformation>> _files;
        private IReadOnlyList<IFileOperation> _operations;

        protected DeleteFilesDecisionViewModel(IFileIssue fileIssue, IReadOnlyList<Checkable<FileInformation>> files)
        {
            Issue = fileIssue;
            _allFiles = files;

            IssueFileCheckable = files.Single(x => x.Value == fileIssue.File);

            _files = GetFilesView(ImmutableList<FileInformation>.Empty);
            _operations = GetOperations();

            foreach (var file in files)
                file.PropertyChanged += FileOnPropertyChanged;
        }

        public Checkable<FileInformation> IssueFileCheckable { get; }

        /// <summary>
        ///     All files (including the one of the issue). Files that are checked won't be deleted
        /// </summary>
        public IReadOnlyList<Checkable<FileInformation>> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        public abstract bool IsRecommended { get; }
        public IFileIssue Issue { get; }

        public IReadOnlyList<IFileOperation> Operations
        {
            get => _operations;
            private set => SetProperty(ref _operations, value);
        }

        public bool UpdateDeletedFiles(IReadOnlyList<FileInformation> deletedFiles)
        {
            Files = GetFilesView(deletedFiles);
            return (!IssueFileCheckable.IsChecked || deletedFiles.All(x => Issue.File.Filename != x.Filename)) && Files.Any();
        }

        private IReadOnlyList<IFileOperation> GetOperations()
        {
            return _files.Where(x => !x.IsChecked).Select(x => new DeleteFileOperation(x.Value)).ToList();
        }

        private void FileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Operations = GetOperations();
        }

        private IReadOnlyList<Checkable<FileInformation>> GetFilesView(IReadOnlyList<FileInformation> deletedFiles)
        {
            return _allFiles.Where(x => !x.IsChecked || deletedFiles.All(y => y.Filename != x.Value.Filename)).ToList();
        }
    }

    public class DuplicateFileDecisionViewModel : DeleteFilesDecisionViewModel
    {
        public DuplicateFileDecisionViewModel(DuplicateFilesIssue fileIssue, IReadOnlyList<Checkable<FileInformation>> files) : base(fileIssue, files)
        {
        }

        public override bool IsRecommended { get; } = true;
    }

    public class SimilarFileDecisionViewModel : DeleteFilesDecisionViewModel
    {
        public SimilarFileDecisionViewModel(SimilarFilesIssue fileIssue, IReadOnlyList<Checkable<FileInformation>> files) : base(fileIssue, files)
        {
        }

        public override bool IsRecommended { get; } = false;
    }

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

        public IFileIssue Issue { get; }
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
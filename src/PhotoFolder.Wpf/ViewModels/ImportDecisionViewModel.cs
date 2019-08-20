using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
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

    public interface IIssueDecisionViewModel
    {
        bool IsRecommended { get; }
        IReadOnlyList<IFileOperation> Operations { get; }
        IFileIssue Issue { get; }

        bool UpdateDeletedFiles(IReadOnlyList<FileInformation> deletedFiles);
    }

    public class IssueDecisionWrapperViewModel : BindableBase
    {
        private bool _execute;

        public bool Execute
        {
            get { return _execute; }
            set => SetProperty(ref _execute, value);
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
            get { return _isChecked; }
            set => SetProperty(ref _isChecked, value);
        }
    }

    public class DuplicateFileDecisionViewModel : BindableBase, IIssueDecisionViewModel
    {
        private IReadOnlyList<IFileOperation> _operations;

        public DuplicateFileDecisionViewModel(IReadOnlyList<Checkable<FileInformation>> files, IFileIssue issue)
        {
            Files = files;
            Issue = issue;

            _operations = GetOperations();
            foreach (var file in files)
                file.PropertyChanged += FileOnPropertyChanged;
        }

        public bool IsRecommended { get; } = true;

        // Checked files won't be deleted
        public IReadOnlyList<Checkable<FileInformation>> Files { get; }

        public IReadOnlyList<IFileOperation> Operations
        {
            get { return _operations; }
            private set => SetProperty(ref _operations, value);
        }

        public IFileIssue Issue { get; }

        private IReadOnlyList<IFileOperation> GetOperations()
        {
            return Files.Where(x => !x.IsChecked).Select(x => new DeleteFileOperation(x.Value)).ToList();
        }

        private void FileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Operations = GetOperations();
        }
    }

    public class SimilarFileDecisionViewModel : BindableBase, IIssueDecisionViewModel
    {
        private IReadOnlyList<IFileOperation> _operations;
        private readonly IReadOnlyList<Checkable<FileInformation>> _allFiles;
        private IReadOnlyList<Checkable<FileInformation>> _files;

        public SimilarFileDecisionViewModel(IReadOnlyList<Checkable<FileInformation>> files, IFileIssue issue)
        {
            _allFiles = files;
            Issue = issue;

            _operations = GetOperations();
            _files = GetFilesView(ImmutableList<FileInformation>.Empty);

            foreach (var file in files)
                file.PropertyChanged += FileOnPropertyChanged;
        }

        public bool IsRecommended { get; } = false;


        // Checked files won't be deleted
        public IReadOnlyList<Checkable<FileInformation>> Files
        {
            get { return _files; }
            set { _files = value; }
        }

        public IReadOnlyList<IFileOperation> Operations
        {
            get { return _operations; }
            private set => SetProperty(ref _operations, value);
        }

        public IFileIssue Issue { get; }

        private IReadOnlyList<IFileOperation> GetOperations()
        {
            return Files.Where(x => !x.IsChecked).Select(x => new DeleteFileOperation(x.Value)).ToList();
        }

        private void FileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Operations = GetOperations();
        }

        public bool UpdateDeletedFiles(IReadOnlyList<FileInformation> deletedFiles)
        {
            Files = GetFilesView(deletedFiles);
            return !deletedFiles.Any(x => Issue.File.Filename != x.Filename) && Files.Any();
        }

        private IReadOnlyList<Checkable<FileInformation>> GetFilesView(IReadOnlyList<FileInformation> deletedFiles)
        {
            return _files.Where(x => !x.IsChecked || !deletedFiles.Any(y => y.Filename == x.Value.Filename)).ToList();
        }
    }

    public class InvalidLocationFileDecisionViewModel : BindableBase, IIssueDecisionViewModel
    {
        public InvalidLocationFileDecisionViewModel(InvalidFileLocationIssue issue)
        {
            Issue = issue;

            Operations = new[] { new MoveFileOperation(issue.File, issue.Suggestions.First().Filename) };
        }

        public bool IsRecommended { get; } = true;

        public IReadOnlyList<IFileOperation> Operations { get; }

        public IFileIssue Issue { get; }

        public bool UpdateDeletedFiles(IEnumerable<FileInformation> deletedFiles)
        {
            return !deletedFiles.Any(x => x.Filename == Issue.File.Filename);
        }
    }
}
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using PhotoFolder.Application.Dto;
using PhotoFolder.Core.Dto.Services;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels.Models
{
    public abstract class DeleteFilesDecisionViewModel : BindableBase, IIssueDecisionViewModel
    {
        private readonly IFileIssue _fileIssue;
        protected readonly IReadOnlyList<Checkable<FileInformation>> _allFiles;
        protected IReadOnlyList<Checkable<FileInformation>> _files;
        private IReadOnlyList<IFileOperation> _operations;

        protected DeleteFilesDecisionViewModel(IFileIssue fileIssue, IReadOnlyList<Checkable<FileInformation>> files)
        {
            _fileIssue = fileIssue;
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
        IFileIssue IIssueDecisionViewModel.Issue => _fileIssue;

        public IReadOnlyList<IFileOperation> Operations
        {
            get => _operations;
            private set => SetProperty(ref _operations, value);
        }

        public bool UpdateDeletedFiles(IReadOnlyList<FileInformation> deletedFiles)
        {
            Files = GetFilesView(deletedFiles);
            return (!IssueFileCheckable.IsChecked || deletedFiles.All(x => _fileIssue.File.Filename != x.Filename)) && Files.Any();
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
}
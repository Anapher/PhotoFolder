using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Core.Interfaces.Gateways;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoFolder.Wpf.ViewModels
{
    public class ImportDecisionViewModel : BindableBase
    {
        private FileAction _fileAction;
        private string? _targetPath;

        public ImportDecisionViewModel(FileInformation fileInformation, CheckFileIntegrityResponse integrity, IPhotoDirectory photoDirectory)
        {

        }

        public string Analysis { get; }
        public FileAction RecommendedFileAction { get; }

        public string? TargetPath
        {
            get { return _targetPath; }
            set => SetProperty(ref _targetPath, value);
        }

        public FileAction FileAction
        {
            get { return _fileAction; }
            set => SetProperty(ref _fileAction, value);
        }

        public string CurrentPath { get; }
    }

    public enum FileAction
    {
        Move,
        Copy,
        Delete,
        None,
        Decide
    }
}

using System.Collections.Generic;
using PhotoFolder.Application.Shared;
using PhotoFolder.Core.Dto;
using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Application.Dto.WorkerStates
{
    public class CheckFilesState : PropertyChangedBase
    {
        private int _filesProcessed;
        private double _progress;
        private CheckFilesStatus _status;
        private int _totalFiles;

        public CheckFilesStatus Status
        {
            get => _status;
            internal set => SetProperty(ref _status, value);
        }

        public double Progress
        {
            get => _progress;
            internal set => SetProperty(ref _progress, value);
        }

        public int TotalFiles
        {
            get => _totalFiles;
            internal set => SetProperty(ref _totalFiles, value);
        }

        public int FilesProcessed
        {
            get => _filesProcessed;
            internal set
            {
                if (SetProperty(ref _filesProcessed, value))
                    Progress = (double) value / _totalFiles;
            }
        }

        public Dictionary<FileInformation, Error> Errors { get; } = new Dictionary<FileInformation, Error>();
    }

    public enum CheckFilesStatus
    {
        Querying
    }
}

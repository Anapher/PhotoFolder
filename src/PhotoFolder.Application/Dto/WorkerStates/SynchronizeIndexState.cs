using PhotoFolder.Application.Shared;
using PhotoFolder.Core.Dto;
using System.Collections.Generic;

namespace PhotoFolder.Application.Dto.WorkerStates
{
    public class SynchronizeIndexState : PropertyChangedBase
    {
        private SynchronizeIndexStatus _status;
        private double _progress;
        private int _processedFiles;
        private int _totalFiles;

        public SynchronizeIndexStatus Status
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

        public int ProcessedFiles
        {
            get => _processedFiles;
            internal set => SetProperty(ref _processedFiles, value);
        }

        public Dictionary<string, Error> Errors { get; } = new Dictionary<string, Error>();
    }

    public enum SynchronizeIndexStatus
    {
        Scanning,
        Synchronizing,
        IndexingNewFiles
    }
}

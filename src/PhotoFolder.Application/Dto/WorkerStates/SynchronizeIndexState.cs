using PhotoFolder.Application.Shared;
using PhotoFolder.Core.Dto;
using System.Collections.Generic;

namespace PhotoFolder.Application.Dto.WorkerStates
{
    public class SynchronizeIndexState : PropertyChangedBase
    {
        private SynchronizeIndexStatus _status;
        private double _progress;

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

        public Dictionary<string, Error> Errors { get; } = new Dictionary<string, Error>();
    }

    public enum SynchronizeIndexStatus
    {
        Scanning,
        Synchronizing,
        IndexingNewFiles
    }
}

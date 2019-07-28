using PhotoFolder.Application.Shared;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Dto;
using System.Collections.Generic;

namespace PhotoFolder.Application.Dto.WorkerStates
{
    public class CheckIndexState : PropertyChangedBase
    {
        private CheckIndexStatus _status;
        private double _progress;

        public CheckIndexStatus Status
        {
            get => _status;
            internal set => SetProperty(ref _status, value);
        }

        public double Progress
        {
            get => _progress;
            internal set => SetProperty(ref _progress, value);
        }

        public Dictionary<FileInformation, Error> Errors { get; } = new Dictionary<FileInformation, Error>();
    }

    public enum CheckIndexStatus
    {
        Querying
    }
}

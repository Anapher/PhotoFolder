using System.Threading;
using PhotoFolder.Application.Shared;

namespace PhotoFolder.Application.Dto.WorkerStates
{
    public class ExecuteOperationsState : PropertyChangedBase
    {
        private int _processedOperations;
        private int _totalOperations;

        public int TotalOperations
        {
            get => _totalOperations;
            internal set => SetProperty(ref _totalOperations, value);
        }

        public int ProcessedOperations
        {
            get => _processedOperations;
            internal set => SetProperty(ref _processedOperations, value);
        }

        internal void OnOperationProcessed()
        {
            Interlocked.Increment(ref _processedOperations);
            OnPropertyChanged(nameof(ProcessedOperations));
        }
    }
}

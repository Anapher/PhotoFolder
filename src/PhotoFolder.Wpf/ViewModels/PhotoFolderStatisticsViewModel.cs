using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Infrastructure.Photos;
using PhotoFolder.Wpf.Services;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class PhotoFolderStatisticsViewModel : BindableBase
    {
        private readonly PhotoFolderSynchronizationEvent _synchronizationEvent;
        private int _totalFiles;
        private long _totalSize;
        private int _totalUniqueFiles;

        public PhotoFolderStatisticsViewModel(PhotoFolderSynchronizationEvent synchronizationEvent)
        {
            _synchronizationEvent = synchronizationEvent;
        }

        public int TotalFiles
        {
            get => _totalFiles;
            private set => SetProperty(ref _totalFiles, value);
        }

        public int TotalUniqueFiles
        {
            get => _totalUniqueFiles;
            private set => SetProperty(ref _totalUniqueFiles, value);
        }

        public long TotalSize
        {
            get => _totalSize;
            private set => SetProperty(ref _totalSize, value);
        }

        public async void Initialize(IPhotoDirectory photoDirectory)
        {
            await ComputeStatistics(photoDirectory);

#pragma warning disable 4014
            _synchronizationEvent.PhotoFolderSynchronized += (sender, args) => ComputeStatistics(photoDirectory);
        }

        private async Task ComputeStatistics(IPhotoDirectory photoDirectory)
        {
            using var context = ((PhotoDirectory)photoDirectory).GetAppDbContext();

            TotalFiles = await context.Set<FileLocation>().CountAsync();
            TotalUniqueFiles = await context.Set<IndexedFile>().CountAsync();
            TotalSize = await context.Set<IndexedFile>().SumAsync(x => x.Length * x.Files.Count());
        }
    }
}
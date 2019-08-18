using System.Linq;
using Microsoft.EntityFrameworkCore;
using PhotoFolder.Core.Domain.Entities;
using PhotoFolder.Core.Interfaces.Gateways;
using PhotoFolder.Infrastructure.Photos;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class PhotoFolderStatisticsViewModel : BindableBase
    {
        private int _totalFiles;
        private long _totalSize;
        private int _totalUniqueFiles;

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
            using var context = ((PhotoDirectory) photoDirectory).GetAppDbContext();

            TotalFiles = await context.Set<FileLocation>().CountAsync();
            TotalUniqueFiles = await context.Set<IndexedFile>().CountAsync();
            TotalSize = await context.Set<IndexedFile>().SumAsync(x => x.Length * x.Files.Count());
        }
    }
}
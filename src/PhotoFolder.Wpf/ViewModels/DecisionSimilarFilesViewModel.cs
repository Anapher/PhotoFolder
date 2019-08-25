using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Core.Dto.UseCaseResponses;
using PhotoFolder.Wpf.Utilities;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionSimilarFilesViewModel : BindableBase
    {
        private SimilarFileDecisionViewModel? _decision;
        private IReadOnlyList<SimilarFileViewModel>? _files;
        private SimilarFilesIssue? _issue;

        public SimilarFileDecisionViewModel? Decision
        {
            get => _decision;
            set => SetProperty(ref _decision, value);
        }

        public SimilarFilesIssue? Issue
        {
            get => _issue;
            set => SetProperty(ref _issue, value);
        }

        public IReadOnlyList<SimilarFileViewModel>? Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        public async void Initialize(DecisionAssistantContext context)
        {
            Decision = (SimilarFileDecisionViewModel) context.DecisionWrapper.Decision;
            Issue = (SimilarFilesIssue) Decision.Issue;

            var images = new Dictionary<FileInformation, BitmapSource>();
            foreach (var file in Decision.Files)
            {
                var image = await ImageUtils.LoadImageAsync(file.Value.Filename);
                images.Add(file.Value, image);
            }

            Files = Decision.Files.Select(x => new SimilarFileViewModel(x, Issue.SimilarFiles.FirstOrDefault(y => y.FileInfo == x.Value), images[x.Value]))
                .OrderBy(x => x.SimilarFile != null).ToList();
        }
    }

    public class SimilarFileViewModel : BindableBase
    {
        public SimilarFileViewModel(Checkable<FileInformation> file, SimilarFile? similarFile, BitmapSource image)
        {
            File = file;
            SimilarFile = similarFile;
            Image = image;
        }

        public Checkable<FileInformation> File { get; }
        public SimilarFile? SimilarFile { get; }
        public BitmapSource Image { get; }
    }
}

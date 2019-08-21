using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionManagerDetailsViewModel : BindableBase
    {
        private IssueDecisionWrapperViewModel? _selection;
        private BitmapSource? _thumbnail;

        public IssueDecisionWrapperViewModel? Selection
        {
            get => _selection;
            set => SetProperty(ref _selection, value);
        }

        public BitmapSource? Thumbnail
        {
            get => _thumbnail;
            set => SetProperty(ref _thumbnail, value);
        }

        public void Initialize(DecisionManagerContext context)
        {
            context.PropertyChanged += ContextOnPropertyChanged;
        }

        private void ContextOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DecisionManagerContext.SelectedIssue))
            {
                var context = (DecisionManagerContext) sender;
                OnSelectedDecisionChanged(context.SelectedIssue);
            }
        }

        public void OnSelectedDecisionChanged(IssueDecisionWrapperViewModel? viewModel)
        {
            Selection = viewModel;

            if (viewModel == null)
            {
                Thumbnail = null;
            }
            else
            {
                Thumbnail = new BitmapImage(new Uri(viewModel.Decision.Issue.File.Filename));
            }
        }
    }
}
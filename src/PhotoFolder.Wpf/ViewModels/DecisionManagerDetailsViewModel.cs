using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionManagerDetailsViewModel : BindableBase
    {
        private IFileDecisionViewModel? _selection;
        private BitmapSource? _thumbnail;

        public IFileDecisionViewModel? Selection
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
            if (e.PropertyName == nameof(DecisionManagerContext.SelectedDecision))
            {
                var context = (DecisionManagerContext) sender;
                OnSelectedDecisionChanged(context.SelectedDecision);
            }
        }

        public void OnSelectedDecisionChanged(IFileDecisionViewModel? viewModel)
        {
            Selection = viewModel;

            if (viewModel == null)
            {
                Thumbnail = null;
            }
            else
            {
                Thumbnail = new BitmapImage(new Uri(viewModel.File.Filename));
            }
        }
    }
}
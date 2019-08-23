using System.Collections.ObjectModel;
using System.ComponentModel;
using PhotoFolder.Core.Dto.Services;
using PhotoFolder.Wpf.ViewModels.Models;
using Prism.Mvvm;

namespace PhotoFolder.Wpf.ViewModels
{
    public class DecisionDuplicateFilesViewModel : BindableBase
    {
        private DuplicateFileDecisionViewModel? _decision;
        private ObservableCollection<Checkable<FileInformation>>? _files;

        public DuplicateFileDecisionViewModel? Decision
        {
            get => _decision;
            set => SetProperty(ref _decision, value);
        }


        public ObservableCollection<Checkable<FileInformation>>? Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        public void Initialize(DecisionAssistantContext context)
        {
            Decision = (DuplicateFileDecisionViewModel) context.DecisionWrapper.Decision;
            Files = new ObservableCollection<Checkable<FileInformation>>(Decision.Files);

            Decision.PropertyChanged += DecisionOnPropertyChanged;
        }

        private void DecisionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Files!.Clear();
            foreach (var file in Decision!.Files)
                Files.Add(file);
        }
    }
}
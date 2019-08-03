using PhotoFolder.Infrastructure.TemplatePath;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace PhotoFolder.Wpf.ViewModels
{
    public class ConfigureFolderDialogViewModel : BindableBase, IDialogAware
    {
        private DelegateCommand? _configureCommand;
        private string _pathTemplate = "{date:yyyy}/{date:MM.dd} - {eventName}/{filename}";

        public string Title { get; } = "Configure your Photo Folder";

        public string PathTemplate
        {
            get { return _pathTemplate; }
            set => SetProperty(ref _pathTemplate, value);
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
            RequestClose?.Invoke(new DialogResult());
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        public DelegateCommand ConfigureCommand
        {
            get
=> _configureCommand ?? (_configureCommand = new DelegateCommand(() =>
{
    var result = new DialogResult(ButtonResult.OK, new DialogParameters { { "templateString", PathTemplate } });
    RequestClose?.Invoke(result);
}, CheckPathTemplate)).ObservesProperty(() => PathTemplate);
        }

        private bool CheckPathTemplate()
        {
            if (string.IsNullOrWhiteSpace(PathTemplate))
                return false;

            try
            {
                TemplateString.Parse(PathTemplate);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}

using System;
using PhotoFolder.Core.Domain.Template;
using PhotoFolder.Wpf.Utilities;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace PhotoFolder.Wpf.ViewModels
{
    public class ConfigureFolderDialogViewModel : DialogBase
    {
        private DelegateCommand? _configureCommand;
        private string _pathTemplate = "{date:yyyy}/{date:MM.dd} - {eventName}/{filename}";

        public string PathTemplate
        {
            get => _pathTemplate;
            set => SetProperty(ref _pathTemplate, value);
        }

        public DelegateCommand ConfigureCommand =>
            _configureCommand ??= new DelegateCommand(() =>
            {
                var result = new DialogResult(ButtonResult.OK, new DialogParameters {{"templateString", PathTemplate}});
                OnRequestClose(result);
            }, CheckPathTemplate).ObservesProperty(() => PathTemplate);

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
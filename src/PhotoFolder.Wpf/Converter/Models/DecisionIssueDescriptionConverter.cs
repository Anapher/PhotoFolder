using System;
using System.Globalization;
using System.Windows.Data;
using Humanizer;
using PhotoFolder.Core.Dto.Services.FileIssue;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.Converter.Models
{
    public class DecisionIssueDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InvalidLocationFileDecisionViewModel invalidLocation)
                return invalidLocation.Issue.File.RelativeFilename != null ? "Wrong File Location" : "Import File";

            if (value is DuplicateFileDecisionViewModel duplicateFile)
                return duplicateFile.Issue.File.RelativeFilename != null ? "Duplicate Files" : "Import Duplicate File";

            if (value is SimilarFileDecisionViewModel similarFile)
                return similarFile.Issue.File.RelativeFilename != null ? "Similar Files" : "Import Similar File";

            if (value is FormerlyDeletedFileDecisionViewModel formerlyDeletedFile)
                return $"File deleted {((FormerlyDeletedIssue) formerlyDeletedFile.Issue).DeletedFileInfo.DeletedAt.Humanize()}";

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
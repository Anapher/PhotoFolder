using System;
using System.Globalization;
using System.Windows.Data;
using PhotoFolder.Wpf.ViewModels.Models;

namespace PhotoFolder.Wpf.Converters
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

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
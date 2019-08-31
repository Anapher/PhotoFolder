using System;
using System.Globalization;
using System.Windows.Data;
using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Wpf.Converter.Models
{
    public class GetFileInformationPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fileInformation = (FileInformation) value;
            if (fileInformation == null) return string.Empty;

            return fileInformation.RelativeFilename ?? fileInformation.Filename;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

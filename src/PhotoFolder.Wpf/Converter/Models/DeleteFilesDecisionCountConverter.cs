using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using PhotoFolder.Application.Dto;
using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Wpf.Converter.Models
{
    public class DeleteFilesDecisionCountConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var operations = (IReadOnlyList<IFileOperation>) values[0];
            var issueFile = (FileInformation) values[1];
            var deleteFilesFromOutside = (bool) values[2];

            if (!deleteFilesFromOutside && issueFile.RelativeFilename == null)
                operations = operations.Where(x => x.File != issueFile).ToList();

            return new {operations.Count, IsImport = issueFile.RelativeFilename == null};
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

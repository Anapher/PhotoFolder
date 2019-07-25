using PhotoFolder.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PhotoFolder.Infrastructure.Photos
{
    public class TemplateString
    {
        public static TemplateString Parse()
        {

        }

        //public static Regex IsPathMatched(string template, FileInformation fileInformation)
        //{

        //}

        //private static string FormatTemplate(FileInformation fileInformation)
        //{

        //}
    }

    public class PlaceholderFragment
    {
        public PlaceholderFragment(string placeholderName)
        {
            PlaceholderName = placeholderName;
        }

        public string PlaceholderName { get; }
    }

    public class TextFragment : ITemplateFragment
    {
        public string Value => throw new NotImplementedException();
    }

    public interface ITemplateFragment
    {
        public string Value { get; }
    }
}
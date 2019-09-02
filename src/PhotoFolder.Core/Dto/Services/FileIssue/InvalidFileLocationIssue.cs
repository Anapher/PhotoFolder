using System.Collections.Generic;
using System.Linq;
using PhotoFolder.Core.Domain.Template;

namespace PhotoFolder.Core.Dto.Services.FileIssue
{
    public class InvalidFileLocationIssue : IFileIssue
    {
        public InvalidFileLocationIssue(FileInformation file, TemplateString directoryPathTemplate, IReadOnlyList<FilenameSuggestion> suggestions, string correctFilename)
        {
            File = file;
            DirectoryPathTemplate = directoryPathTemplate;
            Suggestions = suggestions;
            CorrectFilename = correctFilename;
        }

        public FileInformation File { get; }
        public IEnumerable<FileInformation> RelevantFiles => Enumerable.Empty<FileInformation>();

        public TemplateString DirectoryPathTemplate { get; }
        public IReadOnlyList<FilenameSuggestion> Suggestions { get; }

        public string CorrectFilename { get; }

        public string Identity => "InvalidLocation:" + (File.RelativeFilename ?? File.Filename);
    }

    public class FilenameSuggestion
    {
        public FilenameSuggestion(string directory, string filename)
        {
            Directory = directory;
            Filename = filename;
        }

        public string Directory { get; }
        public string Filename { get; }
    }
}

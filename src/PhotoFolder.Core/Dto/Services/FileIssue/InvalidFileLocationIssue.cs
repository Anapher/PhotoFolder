﻿using PhotoFolder.Core.Extensions;
using System.Collections.Generic;

namespace PhotoFolder.Core.Dto.Services.FileIssue
{
    public class InvalidFileLocationIssue : IFileIssue
    {
        public InvalidFileLocationIssue(FileInformation file, string directoryPathTemplate, IReadOnlyList<FilenameSuggestion> suggestions, string correctFilename)
        {
            File = file;
            DirectoryPathTemplate = directoryPathTemplate;
            Suggestions = suggestions;
            CorrectFilename = correctFilename;
        }

        public FileInformation File { get; }
        public IEnumerable<FileInformation> RelevantFiles => File.Yield();

        public string DirectoryPathTemplate { get; }
        public IReadOnlyList<FilenameSuggestion> Suggestions { get; }

        public string CorrectFilename { get; }

        public string Identity => File.Filename;
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
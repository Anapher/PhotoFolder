#pragma warning disable CS8618 // Non-nullable field is uninitialized.

using System;

namespace PhotoFolder.Infrastructure.Photos
{
    public class PhotoDirectoryConfig
    {
        public PhotoDirectoryConfig(string templatePath)
        {
            TemplatePath = templatePath;
            Guid = Guid.NewGuid();
        }

        private PhotoDirectoryConfig()
        {
        }

        public string TemplatePath { get; set; }
        public Guid Guid { get; set; }
    }
}

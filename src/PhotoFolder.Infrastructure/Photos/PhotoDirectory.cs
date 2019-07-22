using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;

namespace PhotoFolder.Infrastructure.Photos
{
    public class PhotoDirectory
    {
        private readonly IFileSystem _fileSystem;

        public PhotoDirectory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }


    }
}

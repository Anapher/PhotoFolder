using System;

namespace PhotoFolder.Core.Dto.Services
{
    public interface IFileInfo
    {
        string Filename { get; }
        long Length { get; }
        DateTimeOffset CreatedOn { get; }
        DateTimeOffset ModifiedOn { get; }
    }
}

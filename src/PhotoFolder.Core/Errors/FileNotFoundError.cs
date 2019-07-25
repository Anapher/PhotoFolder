namespace PhotoFolder.Core.Errors
{
    public class FileNotFoundError : DomainError
    {
        public FileNotFoundError(string filename) : base(ErrorType.InvalidOperation, $"The file {filename} was not found.", ErrorCode.FileNotFound)
        {
        }
    }
}

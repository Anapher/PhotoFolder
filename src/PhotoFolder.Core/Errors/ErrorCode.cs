namespace PhotoFolder.Core.Errors
{
    public enum ErrorCode
    {
        FieldValidation = 0,
        FileNotFound,
        FileAlreadyIndexed,
        FileNotIndexed,
        FileNotInPhotoDirectory,
        PathFullyQualified
    }
}

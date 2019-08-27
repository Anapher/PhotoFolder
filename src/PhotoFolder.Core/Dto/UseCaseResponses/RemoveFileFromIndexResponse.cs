namespace PhotoFolder.Core.Dto.UseCaseResponses
{
    public class RemoveFileFromIndexResponse
    {
        public RemoveFileFromIndexResponse(bool isCompletelyRemoved)
        {
            IsCompletelyRemoved = isCompletelyRemoved;
        }

        /// <summary>
        ///     True if that file was completely removed from the folder, so it also doesn't exist under a different name
        /// </summary>
        public bool IsCompletelyRemoved { get; }
    }
}

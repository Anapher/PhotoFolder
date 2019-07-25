using System.Collections.Generic;

namespace PhotoFolder.Core.Errors
{
    public class InvalidOperationError : DomainError
    {
        public InvalidOperationError(string message, ErrorCode code, IReadOnlyDictionary<string, string>? fields = null)
            : base(ErrorType.InvalidOperation, message, code, fields)
        {
        }
    }
}

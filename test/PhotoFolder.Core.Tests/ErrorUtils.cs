using PhotoFolder.Core.Errors;
using PhotoFolder.Core.Interfaces;
using Xunit;

namespace PhotoFolder.Core.Tests
{
    public static class ErrorUtils
    {
        public static void AssertError(IUseCaseErrors errors, ErrorType? errorType = null, ErrorCode? code = null)
        {
            Assert.True(errors.HasError);

            if (errorType != null)
                Assert.Equal(errorType.ToString(), errors.Error.Type);

            if (code != null)
                Assert.Equal((int) code, errors.Error.Code);
        }
    }
}

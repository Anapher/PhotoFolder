using PhotoFolder.Core.Domain;
using PhotoFolder.Core.Dto.Services;

namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IBitmapHashComparer
    {
        HashContext CreateContext(Hash hash);

        float Compare(HashContext context, Hash bitmapHash);
    }
}

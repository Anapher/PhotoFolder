namespace PhotoFolder.Core.Interfaces.Services
{
    public interface IBitmapHashComparer
    {
        float RequiredBitmapHashEquality { get; }

        float Compare(Hash x, Hash y);
    }
}

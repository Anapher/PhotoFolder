using System;

namespace PhotoFolder.Core.Dto.Services
{
    public struct HashContext
    {
        public HashContext(ReadOnlyMemory<byte> colorData, ReadOnlyMemory<byte>[] variants)
        {
            ColorData = colorData;
            Variants = variants;
        }

        public ReadOnlyMemory<byte> ColorData { get; }
        public ReadOnlyMemory<byte>[] Variants { get; }
    }
}

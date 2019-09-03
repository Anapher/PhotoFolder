using System.Buffers;
using System.IO;

namespace PhotoFolder.Infrastructure.Utilities
{
    public class PooledMemoryStream : MemoryStream
    {
        private readonly byte[] _buffer;
        private readonly ArrayPool<byte> _pool;

        public PooledMemoryStream(byte[] buffer, int index, int count, ArrayPool<byte> pool) : base(buffer, index, count, false)
        {
            _buffer = buffer;
            _pool = pool;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pool.Return(_buffer);
            }
            base.Dispose(disposing);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCore
{
    internal class RecvBuffer
    {
        private byte[] _buffer;
        private int _readPos;
        private int _writePos;

        public RecvBuffer(int bufferSize = 8192)
        {
            _buffer = new byte[bufferSize];
        }

        public int DataSize => _writePos - _readPos;

        public int FreeSize => _buffer.Length - _writePos;

        public Span<byte> ReadSpan => new Span<byte>(_buffer, _readPos, DataSize);

        public Memory<byte> WriteMemory => _buffer.AsMemory(_writePos, FreeSize);

        public void OnRead(int bytes) => _readPos += bytes;

        public void OnWrite(int bytes) => _writePos += bytes;

        public void Clear()
        {
            int dataSize = DataSize;

            if (dataSize == 0)
            {
                _readPos = _writePos = 0;
            }
            else
            {
                Array.Copy(_buffer, _readPos, _buffer, 0, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCore
{
    public class PacketWriter : IDisposable
    {
        private MemoryStream _memoryStream;
        private BinaryWriter _binaryWriter;

        public PacketWriter()
        {
            // memory pool 사용 고려
            _memoryStream = new MemoryStream();
            _binaryWriter = new BinaryWriter(_memoryStream, Encoding.UTF8, true);
        }

        public void Dispose()
        {
            _binaryWriter?.Dispose();
            _memoryStream?.Dispose();
        }

        public byte[] ToArray() => _memoryStream.ToArray();

        public void Write(byte value) => _binaryWriter.Write(value);
        public void Write(short value) => _binaryWriter.Write(value);
        public void Write(ushort value) => _binaryWriter.Write(value);
        public void Write(int value) => _binaryWriter.Write(value);
        public void Write(uint value) => _binaryWriter.Write(value);
        public void Write(long value) => _binaryWriter.Write(value);
        public void Write(ulong value) => _binaryWriter.Write(value);
        public void Write(float value) => _binaryWriter.Write(value);
        public void Write(double value) => _binaryWriter.Write(value);
        public void Write(bool value) => _binaryWriter.Write(value);
        public void Write(string value) => _binaryWriter.Write(value);
        public void Write(byte[] value)
        {
            _binaryWriter.Write((short)value.Length);
            _binaryWriter.Write(value);
        }
        public void Write(Span<byte> value)
        {
            _binaryWriter.Write((short)value.Length);
            _binaryWriter.Write(value.ToArray());
        }
        public void Write(List<string> value)
        {
            _binaryWriter.Write((short)value.Count);

            foreach (var item in value)
                _binaryWriter.Write(item);
        }
    }
}

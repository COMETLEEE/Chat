using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCore
{
    public class PacketReader : IDisposable
    {
        MemoryStream _memoryStream;
        BinaryReader _binaryReader;

        public PacketReader(byte[] data)
        {
            _memoryStream = new MemoryStream(data);
            _binaryReader = new BinaryReader(_memoryStream, Encoding.UTF8, true);
        }

        public void Dispose()
        {
            _binaryReader?.Dispose();
            _memoryStream?.Dispose();
        }

        public byte ReadByte() => _binaryReader.ReadByte();
        public short ReadInt16() => _binaryReader.ReadInt16();
        public ushort ReadUInt16() => _binaryReader.ReadUInt16();
        public int ReadInt32() => _binaryReader.ReadInt32();
        public uint ReadUInt32() => _binaryReader.ReadUInt32();
        public long ReadInt64() => _binaryReader.ReadInt64();
        public ulong ReadUInt64() => _binaryReader.ReadUInt64();
        public float ReadFloat() => _binaryReader.ReadSingle();
        public double ReadDouble() => _binaryReader.ReadDouble();
        public bool ReadBool() => _binaryReader.ReadBoolean();
        public string ReadString() => _binaryReader.ReadString();
        public byte[] ReadBytes(short length) => _binaryReader.ReadBytes(length);
        public byte[] ReadBytes()
        {
            short length = _binaryReader.ReadInt16();
            return ReadBytes(length);
        }
        public List<string> ReadStrings()
        {
            short length = _binaryReader.ReadInt16();

            List<string> strings = new List<string>(length);

            for (int i = 0; i <length; i++)
            {
                string s = _binaryReader.ReadString();
                strings.Add(s);
            }

            return strings;
        }
        public List<uint> ReadUInt32s()
        {
            short length = _binaryReader.ReadInt16();

            List<uint> uints = new List<uint>(length);

            for (int i = 0; i <length; i++)
            {
                uint u = _binaryReader.ReadUInt32();
                uints.Add(u);
            }

            return uints;
        }
    }
}

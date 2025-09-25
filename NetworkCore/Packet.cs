using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCore
{
    public enum PacketType : short
    {
        CREATE_ROOM_REQ = 1,
        CREATE_ROOM_RES = 2,

        EXIT_REQ = 100,
    }

    public static class PacketSerializer
    {
        private const int _headerSize = 4; // 2 bytes for size, 2 bytes for type

        public static byte[] Serialize<T>(PacketType type, T packet) where T : struct
        {
            int bodySize = Marshal.SizeOf(typeof(T));
            int totalSize = bodySize + _headerSize;

            byte[] buffer = new byte[totalSize];

            BinaryPrimitives.WriteInt16LittleEndian(buffer.AsSpan(0, 2), (short)type);
            BinaryPrimitives.WriteInt16LittleEndian(buffer.AsSpan(2, 2), (short)bodySize);

            MemoryMarshal.Write(buffer.AsSpan(4), in packet);

            return buffer;
        }

        public static T Deserialize<T>(byte[] body) where T : struct
        {
            T packet = MemoryMarshal.Read<T>(body.AsSpan(0, Marshal.SizeOf(typeof(T))));

            return packet;
        }
    }
}

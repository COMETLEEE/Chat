using NetworkCore;
using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

using Packet;

namespace ChatServer
{
    internal class ClientSession : NetworkCore.Session
    {
        public Room? CurrentRoom { get; set; }

        public ClientSession(TcpClient tcpClient) : base(tcpClient)
        {
            
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"[ClientSession] OnConnected: {SessionId.Id}");
        }

        protected override void OnDisconnected() 
        { 
            Console.WriteLine($"[ClientSession] OnDisconnected: {SessionId.Id}");
        }

        protected override async Task OnRecv(short type, byte[] body)
        {
            PacketType packetType = (Packet.PacketType)type;

            switch (packetType)
            {
                case PacketType.CreateRoomReq:
                    {
                        CreateRoomReq createRoomReq = PacketSerializer.Deserialize_CreateRoomReq(body);
                        Room room = RoomManager.Instance.CreateRoom(createRoomReq.roomName);

                        CreateRoomRes createRoomRes = new CreateRoomRes() { result = 1, roomId = room.RoomId };

                        // 논리적 패킷 전송 순서가 보장되어야 하므로 기다린다.
                        await SendAsync((short)PacketType.CreateRoomRes, PacketSerializer.Serialize(createRoomRes));
                    }
                    break;

                case PacketType.RoomListReq:
                    {
                        Dictionary<UniqueId<Room>, Room> rooms = RoomManager.Instance.Rooms;

                        RoomListRes roomListRes = new RoomListRes();

                        foreach (var room in rooms.Values)
                        {
                            roomListRes.roomList.Add($"> {room.RoomId} - {room.RoomName}");
                        }

                        await SendAsync((short)PacketType.RoomListRes, PacketSerializer.Serialize(roomListRes));
                    }
                    break;
            }
        }

        protected override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"[ClientSession] OnSend: {SessionId.Id}, {numOfBytes} bytes");
        }
    }
}

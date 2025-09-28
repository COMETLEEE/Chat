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
            Console.WriteLine($"[ClientSession] OnConnected: {SessionId}");
        }

        protected override void OnDisconnected() 
        { 
            Console.WriteLine($"[ClientSession] OnDisconnected: {SessionId}");

            CurrentRoom?.RemoveSession(this);
            CurrentRoom = null;
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

                        await SendAsync((short)PacketType.CreateRoomRes, PacketSerializer.Serialize(createRoomRes));
                    }
                    break;

                case PacketType.RoomListReq:
                    {
                        RoomListRes roomListRes = new RoomListRes();

                        Dictionary<UniqueId<Room>, Room> rooms = RoomManager.Instance.Rooms;

                        foreach (var room in rooms.Values)
                        {
                            roomListRes.roomList.Add($"{room.RoomId} - {room.RoomName}");
                        }

                        await SendAsync((short)PacketType.RoomListRes, PacketSerializer.Serialize(roomListRes));
                    }
                    break;

                case PacketType.RoomEnterReq:
                    {
                        RoomEnterReq roomEnterReq = PacketSerializer.Deserialize_RoomEnterReq(body);

                        Room? room = RoomManager.Instance.GetRoom(roomEnterReq.roomId);

                        byte result = 0;

                        if (room != null)
                        {
                            CurrentRoom = room;
                            room.AddSession(this);
                            result = 1;
                        }
                        else
                        {
                            Console.WriteLine($"해당 번호의 방이 존재하지 않아 들어갈 수 없습니다.");
                            Console.WriteLine($"SESSION ID - {this.SessionId} / REQUESTED ROOM ID - {roomEnterReq.roomId}");
                        }

                        RoomEnterRes roomEnterRes = new RoomEnterRes();
                        roomEnterRes.result = result;
                        await SendAsync((short)PacketType.RoomEnterRes, PacketSerializer.Serialize(roomEnterRes));
                    }
                    break;

                case PacketType.ChatReq:
                    {
                        ChatReq chatReq = PacketSerializer.Deserialize_ChatReq(body);

                        ChatRes chatRes = new ChatRes() { result = 1 };

                        if (CurrentRoom != null)
                        {
                            ChatDataNoti chatDataNoti = new ChatDataNoti() { senderId = this.SessionId, chatData = chatReq.chatData };

                            // Broadcast
                            foreach (var session in CurrentRoom?.Sessions!)
                            {
                                // 여기서 각 Session 의 SendAsync 에 대해 기다릴 필요는 없다.
                                // 만약, 추가적인 예외 처리가 필요하다면 Task.Run 으로 래핑해서 사용하면 된다.
                                _ = session.SendAsync((short)PacketType.ChatDataNoti, PacketSerializer.Serialize(chatDataNoti));
                            }
                        }
                        else
                        {
                            chatRes.result = 0;
                        }

                        await SendAsync((short)PacketType.ChatRes, PacketSerializer.Serialize(chatRes));
                    }
                    break;
            }
        }

        protected override void OnSend(int numOfBytes)
        {
            // Console.WriteLine($"[ClientSession] OnSend: {SessionId}, {numOfBytes} bytes");
        }
    }
}

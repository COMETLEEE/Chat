using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    internal class ServerSession : NetworkCore.Session
    {
        public ServerSession(TcpClient client) : base(client)
        {
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"[ServerSession] OnConnected: {SessionId.Id}");
            Console.WriteLine("- 명령어를 입력하세요.");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"[ServerSession] OnDisconnected: {SessionId.Id}");
        }

        protected override Task OnRecv(short type, byte[] body)
        {
            Packet.PacketType packetType = (Packet.PacketType)type;

            switch (packetType)
            {
                case PacketType.CreateRoomRes:
                    {
                        CreateRoomRes createRoomRes = PacketSerializer.Deserialize_CreateRoomRes(body);

                        if (createRoomRes.result != 0)
                        {
                            Console.WriteLine($"룸 생성에 성공하였습니다. ROOM ID - {createRoomRes.roomId}");
                        }
                        else
                        {
                            Console.WriteLine($"룸 생성에 실패하였습니다.");
                        }
                    }
                    break;

                case PacketType.RoomListRes:
                    {
                        RoomListRes roomListRes = PacketSerializer.Deserialize_RoomListRes(body);

                        Console.WriteLine("=============== 현재 방 정보 ===============");
                        roomListRes.roomList.ForEach((st) => { Console.WriteLine(st); });
                        Console.WriteLine("==========================================");
                    }
                    break;
            }

            return Task.CompletedTask;
        }

        protected override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"[ServerSession] OnSend: {SessionId.Id}, {numOfBytes} bytes");
        }
    }
}

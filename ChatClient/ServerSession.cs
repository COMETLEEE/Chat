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
            Console.WriteLine($"[ServerSession] OnConnected: {SessionId}");
            Console.WriteLine("- 명령어를 입력하세요.");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"[ServerSession] OnDisconnected: {SessionId}");
        }

        protected override Task OnRecv(short type, byte[] body)
        {
            Packet.PacketType packetType = (Packet.PacketType)type;

            switch (packetType)
            {
                case PacketType.DisconnectRes:
                    {
                        DisconnectRes disconnectRes = PacketSerializer.Deserialize_DisconnectRes(body);

                        Console.WriteLine($"서버와의 연결을 종료합니다. 종료 코드 - {disconnectRes.disconnectReason}");

                        Disconnect();
                    }
                    break;

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
                        Console.WriteLine("===========================================");
                    }
                    break;

                case PacketType.RoomEnterRes:
                    {
                        RoomEnterRes roomEnterRes = PacketSerializer.Deserialize_RoomEnterRes(body);

                        if (roomEnterRes.result != 0)
                        {
                            Console.WriteLine($"룸 입장에 성공하였습니다. ");

                            ChatClient.ClientState = ClientState.Room;
                        }
                        else
                        {
                            Console.WriteLine($"룸 입장에 실패하였습니다.");
                        }
                    }
                    break;

                case PacketType.ChatRes:
                    {
                        ChatRes chatRes = PacketSerializer.Deserialize_ChatRes(body);

                        if (chatRes.result == 0)
                        {
                            Console.WriteLine("채팅 전송에 실패하였습니다.");
                        }
                    }
                    break;

                case PacketType.RoomInfoNoti:
                    {
                        RoomInfoNoti roomInfoNoti = PacketSerializer.Deserialize_RoomInfoNoti(body);

                        // 방 정보 계속 갱신
                    }
                    break;

                case PacketType.ChatDataNoti:
                    {
                        ChatDataNoti chatDataNoti = PacketSerializer.Deserialize_ChatDataNoti(body);

                        Console.WriteLine($"{chatDataNoti.senderId} : {chatDataNoti.chatData}");
                    }
                    break;
            }

            return Task.CompletedTask;
        }

        protected override void OnSend(int numOfBytes)
        {
            // Console.WriteLine($"[ServerSession] OnSend: {SessionId}, {numOfBytes} bytes");
        }
    }
}

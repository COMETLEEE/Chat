using ChatClient;
using NetworkCore;
using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetworkCore;
using ChatClient;

namespace Packet
{
    public static partial class PacketHandler
    {
        private static async Task OnDisconnectRes(Session session, DisconnectRes packet)
        {
            Console.WriteLine($"서버와의 연결을 종료합니다. 종료 코드 - {packet.disconnectReason}");

            session.Disconnect();
        }

        private static async Task OnCreateRoomRes(Session session, CreateRoomRes packet)
        {
            if (packet.result != 0)
            {
                Console.WriteLine($"룸 생성에 성공하였습니다. ROOM ID - {packet.roomId}");
            }
            else
            {
                Console.WriteLine($"룸 생성에 실패하였습니다.");
            }
        }

        private static async Task OnRoomListRes(Session session, RoomListRes packet)
        {
            Console.WriteLine("=============== 현재 방 정보 ===============");
            packet.roomList.ForEach((st) => { Console.WriteLine(st); });
            Console.WriteLine("===========================================");
        }

        private static async Task OnRoomEnterRes(Session session, RoomEnterRes packet)
        {
            if (packet.result != 0)
            {
                Console.WriteLine($"룸 입장에 성공하였습니다. ");

                ChatClient.ChatClient.ClientState = ClientState.Room;
            }
            else
            {
                Console.WriteLine($"룸 입장에 실패하였습니다.");
            }
        }

        private static async Task OnChatRes(Session session, ChatRes packet)
        {
            if (packet.result == 0)
            {
                Console.WriteLine("채팅 전송에 실패하였습니다.");
            }
        }

        private static async Task OnRoomInfoNoti(Session session, RoomInfoNoti packet)
        {
            // TODO : 방 정보 계속 갱신 (UI, ...)
        }

        private static async Task OnChatDataNoti(Session session, ChatDataNoti packet)
        {
            Console.WriteLine($"{packet.senderId} : {packet.chatData}");
        }
    }
}

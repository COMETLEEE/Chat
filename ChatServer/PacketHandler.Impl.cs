using ChatServer;
using NetworkCore;
using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packet
{
    public static partial class PacketHandler
    {
        private static async Task OnDisconnectReq(Session session, DisconnectReq packet)
        {
            DisconnectRes disconnectRes = new DisconnectRes() { disconnectReason = packet.disconnectReason };
            await session.SendAsync((short)PacketType.DisconnectRes, PacketSerializer.Serialize(disconnectRes));
        }

        private static async Task OnCreateRoomReq(Session session, CreateRoomReq packet)
        {
            Room room = RoomManager.Instance.CreateRoom(packet.roomName);

            CreateRoomRes createRoomRes = new CreateRoomRes() { result = 1, roomId = room.RoomId };

            await session.SendAsync((short)PacketType.CreateRoomRes, PacketSerializer.Serialize(createRoomRes));
        }

        private static async Task OnRoomListReq(Session session, RoomListReq packet)
        {
            RoomListRes roomListRes = new RoomListRes();

            Dictionary<UniqueId<Room>, Room> rooms = RoomManager.Instance.Rooms;

            foreach (var room in rooms.Values)
            {
                roomListRes.roomList.Add($"{room.RoomId} - {room.RoomName}");
            }

            await session.SendAsync((short)PacketType.RoomListRes, PacketSerializer.Serialize(roomListRes));
        }

        private static async Task OnRoomEnterReq(Session session, RoomEnterReq packet)
        {
            ClientSession? clientSession = session as ClientSession; 
            Room? room = RoomManager.Instance.GetRoom(packet.roomId);

            byte result = 0;

            if (room != null)
            {
                clientSession!.CurrentRoom = room;
                room.AddSession(session);
                result = 1;
            }
            else
            {
                Console.WriteLine($"해당 번호의 방이 존재하지 않아 들어갈 수 없습니다.");
                Console.WriteLine($"SESSION ID - {session.SessionId} / REQUESTED ROOM ID - {packet.roomId}");
            }

            RoomEnterRes roomEnterRes = new RoomEnterRes();
            roomEnterRes.result = result;
            await session.SendAsync((short)PacketType.RoomEnterRes, PacketSerializer.Serialize(roomEnterRes));
        }

        private static async Task OnChatReq(Session session, ChatReq packet)
        {
            ClientSession? clientSession = session as ClientSession;

            ChatRes chatRes = new ChatRes() { result = 1 };

            if (clientSession!.CurrentRoom != null)
            {
                ChatDataNoti chatDataNoti = new ChatDataNoti() { senderId = session.SessionId, chatData = packet.chatData };

                foreach (var roomSession in clientSession!.CurrentRoom?.Sessions!)
                {
                    // 여기서 각 Session 의 SendAsync 에 대해 기다릴 필요는 없다.
                    // 만약, 추가적인 예외 처리가 필요하다면 Task.Run 으로 래핑해서 사용하면 된다.
                    _ = roomSession.SendAsync((short)PacketType.ChatDataNoti, PacketSerializer.Serialize(chatDataNoti));
                }
            }
            else
            {
                chatRes.result = 0;
            }

            await session.SendAsync((short)PacketType.ChatRes, PacketSerializer.Serialize(chatRes));
        }
    }
}

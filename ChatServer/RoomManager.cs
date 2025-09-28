using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NetworkCore;

namespace ChatServer
{
    internal class RoomManager : Singleton<RoomManager>
    {
        private ConcurrentDictionary<UniqueId<Room>, Room> _rooms = new ConcurrentDictionary<UniqueId<Room>, Room>();
        
        public Dictionary<UniqueId<Room>, Room> Rooms => _rooms.ToDictionary();

        public RoomManager()
        {

        }

        public Room CreateRoom(string roomName)
        {
            Room room = new Room(roomName);

            if (false == _rooms.TryAdd(room.RoomId, room))
            {
                Console.WriteLine($"[RoomManager.CreateRoom] 룸 생성에 실패하였습니다. ROOM ID - {room.RoomId}");
            }

            return room;
        }

        public void RemoveRoom(Room room)
        {
            if (_rooms.ContainsKey(room.RoomId))
            {
                _rooms.Remove(room.RoomId, out _);
            }
        }

        public Room? GetRoom(UniqueId<Room> roomId)
        {
            if (_rooms.TryGetValue(roomId, out Room? room))
            {
                return room;
            }

            return null;
        }
    }
}

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
        
        public RoomManager()
        {

        }


        public Room CreateRoom(string roomName)
        {
            Room room = new Room(roomName);

            return room;
        }

        public void RemoveRoom(Room room)
        {
            
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

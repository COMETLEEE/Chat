using NetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ChatServer
{
    internal class Room
    {
        private string _roomName;

        public UniqueId<Room> RoomId { get; private set; }
        
        public Room(string roomName)
        {
            _roomName = roomName;
        }
    }
}

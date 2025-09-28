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

        private UniqueId<Room> _roomId = new UniqueId<Room>();

        public uint RoomId => _roomId.Id;
        
        public string RoomName => _roomName;

        public Room(string roomName)
        {
            _roomName = roomName;
        }
    }
}

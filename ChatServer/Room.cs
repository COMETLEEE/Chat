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
        public Room()
        {

        }

        List<ClientSession> ClientSessions = new List<ClientSession>();

        public UniqueId<Room> RoomId { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class RoomManager : Singleton<RoomManager>
    {
        public RoomManager()
        {

        }


        public Room CreateRoom()
        {
            Room room = new Room();

            return room;
        }

        public void RemoveRoom(Room room)
        {

        }


    }
}

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
        private UniqueId<Room> _roomId = new UniqueId<Room>();
        private string _roomName;
        private List<Session> _sessions = new List<Session>();
        private object _sessionsLock = new object();

        public uint RoomId => _roomId.Id;
        public string RoomName => _roomName;
        public List<Session> Sessions { get 
            {
                lock (_sessionsLock)
                {
                    // _sessions 를 순회 중
                    // 다른 쓰레드에서 _sessions 에 Write 가 들어올 수 있으므로
                    // 복사하여 반환한다.
                    return new List<Session>(_sessions);
                }
            } 
        }

        public Room(string roomName)
        {
            _roomName = roomName;
        }

        public void RemoveSession(Session session)
        {
            lock (_sessionsLock)
            {
                _sessions.Remove(session);
            }
        }

        public void AddSession(Session session)
        {
            if (false == _sessions.Contains(session))
            {
                lock (_sessions)
                {
                    _sessions.Add(session);
                }
            }
        }
    }
}

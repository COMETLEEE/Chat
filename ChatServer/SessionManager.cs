using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using NetworkCore;

namespace ChatServer
{
    internal class SessionManager : Singleton<SessionManager>
    {
        private ConcurrentDictionary<UniqueId<Session>, NetworkCore.Session> _sessions 
            = new ConcurrentDictionary<UniqueId<Session>, NetworkCore.Session>();

        public ClientSession? RegisterClient(TcpClient tcpClient)
        {
            ClientSession clientSession = new ClientSession(tcpClient);

            if (false == _sessions.TryAdd(clientSession.SessionId, clientSession))
            {
                Console.WriteLine($"[SessionManager] RegisterClient() failed. SessionId: {clientSession.SessionId.Id}");

                return null;
            }

            return clientSession;
        }

        public ClientSession? GetClientSession(NetworkCore.UniqueId<Session> sessionId)
        {
            if (_sessions.TryGetValue(sessionId, out NetworkCore.Session? session))
            {
                return session as ClientSession;
            }

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    internal class ServerSession : NetworkCore.Session
    {
        public ServerSession(TcpClient client) : base(client)
        {
        }

        protected override void OnConnected()
        {
            Console.WriteLine($"[ServerSession] OnConnected: {SessionId.Id}");
            Console.WriteLine("- 명령어를 입력하세요.");
        }

        protected override void OnDisconnected()
        {
            Console.WriteLine($"[ServerSession] OnDisconnected: {SessionId.Id}");
        }

        protected override Task OnRecv(NetworkCore.PacketType type, byte[] body)
        {
            switch (type)
            {

            }

            return Task.CompletedTask;
        }

        protected override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"[ServerSession] OnSend: {SessionId.Id}, {numOfBytes} bytes");
        }
    }
}

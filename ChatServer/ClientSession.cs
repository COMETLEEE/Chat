using NetworkCore;
using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

using Packet;

namespace ChatServer
{
    internal class ClientSession : NetworkCore.Session
    {
        public Room? CurrentRoom { get; set; }

        public ClientSession(TcpClient tcpClient) : base(tcpClient)
        {

        }

        protected override void OnConnected()
        {
            Console.WriteLine($"[ClientSession] OnConnected: {SessionId}");
        }

        protected override void OnDisconnected() 
        { 
            Console.WriteLine($"[ClientSession] OnDisconnected: {SessionId}");

            CurrentRoom?.RemoveSession(this);
            CurrentRoom = null;
        }

        protected override async Task OnRecv(short type, byte[] body)
        {
            await PacketHandler.HandlePacket(this, (Packet.PacketType)type, body);
        }

        protected override void OnSend(int numOfBytes)
        {
            // Console.WriteLine($"[ClientSession] OnSend: {SessionId}, {numOfBytes} bytes");
        }
    }
}

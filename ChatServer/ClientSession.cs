using NetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

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
            Console.WriteLine($"[ClientSession] OnConnected: {SessionId.Id}");
        }

        protected override void OnDisconnected() 
        { 
            Console.WriteLine($"[ClientSession] OnDisconnected: {SessionId.Id}");
        }

        protected override Task OnRecv(PacketType type, byte[] body)
        {
            switch (type)
            {
                case PacketType.CREATE_ROOM_REQ:
                    {
                        string message = System.Text.Encoding.UTF8.GetString(body);

                        string roomName = string.Empty;

                        if (false == string.IsNullOrEmpty(message))
                            roomName = message;

                        RoomManager.Instance.CreateRoom(roomName);
                    }
                    break;
            }

            return Task.CompletedTask;
        }

        protected override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"[ClientSession] OnSend: {SessionId.Id}, {numOfBytes} bytes");
        }
    }
}

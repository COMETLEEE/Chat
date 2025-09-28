using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class Listener
    {
        private TcpListener _tcpListener;

        public Listener(ServerConfig serverConfig)
        {
            _tcpListener = new TcpListener(System.Net.IPAddress.Any, serverConfig.ListenPort);
        }

        public void Start()
        {
            _tcpListener.Start();
        }

        public async Task<TcpClient> AcceptTcpClientAsync()
        {
            return await _tcpListener.AcceptTcpClientAsync();
        }
    }
}

using System.Net.Sockets;

namespace ChatServer
{
    internal class ChatServer
    {
        static private bool s_isRunning = true;

        static async Task Main(string[] args)
        {
            Console.WriteLine("------ Start ChatServer ------");

            ServerConfig serverConfig = new ServerConfig()
            {
                ListenPort = 9532
            };

            Listener listener = new Listener(serverConfig);

            listener.Start();

            while (s_isRunning)
            {
                TcpClient tcpClient = await listener.AcceptTcpClientAsync();

                ClientSession? session = SessionManager.Instance.RegisterClient(tcpClient);

                _ = session?.RunAsync();
            }
            
            Console.WriteLine("------ End ChatServer ------");
        }
    }
}

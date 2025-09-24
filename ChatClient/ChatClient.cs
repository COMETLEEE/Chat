using System.Net.Sockets;

namespace ChatClient
{
    internal class ChatClient
    {
        private static bool s_isRunning = true;

        static async Task Main(string[] args)
        {
            Console.WriteLine("------ Start ChatClient------");

            if (args.Length != 2)
            {
                Console.WriteLine("Usage: ChatClient <server_ip> <server_port>");
                return;
            }

            string serverIp = args[0];
            string serverPort = args[1];

            TcpClient tcpClient = new TcpClient();

            try
            {
                System.Net.IPEndPoint endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(serverIp), int.Parse(serverPort));

                await tcpClient.ConnectAsync(endPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: Failed to connect to server.");
                Console.WriteLine($"IP : {serverIp}");
                Console.WriteLine($"Port : {serverPort}");
                Console.WriteLine($"Exeption message : {e.ToString()}");
                return;
            }

            ServerSession session = new ServerSession(tcpClient);

            Task networkTask = session.RunAsync();
            
            Console.WriteLine("------ End ChatClient ------");

            networkTask.Wait();
        }
    }
}

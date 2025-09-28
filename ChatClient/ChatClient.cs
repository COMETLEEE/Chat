using System.Net.Sockets;

namespace ChatClient
{
    enum ClientState : byte
    {
        Lobby,
        Room
    }

    internal class ChatClient
    {
        private static ClientState _clientState = ClientState.Lobby;

        public static ClientState ClientState { get { return _clientState; } set { _clientState = value; } }

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

            Input input = new Input(session);

            Task inputTask = input.RunAsync();

            // 콘솔 창이 닫히거나 프로그램이 종료된 경우 대비
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                session.Disconnect();
            };

            await Task.WhenAll(networkTask, inputTask);

            Console.WriteLine("------ End ChatClient ------");
        }
    }
}

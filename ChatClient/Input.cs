using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient
{
    enum InputState : byte
    {
        Lobby,
        Room
    }

    internal class Input
    {
        private ServerSession _session;

        public Input(ServerSession session)
        {
            _session = session;
            _inputState = InputState.Lobby;
        }

        private InputState _inputState;

        public async Task RunAsync()
        {
            while (false == _session.IsClosed)
            {
                string? line = Console.ReadLine();
                if (true == string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("/quit"))
                {
                    await _session.DisconnectAsync(NetworkCore.PacketType.EXIT_REQ, Encoding.UTF8.GetBytes("CLIENT INPUT EXIT"));
                    break;
                }

                if (_inputState == InputState.Lobby)
                {
                    await ProcessLobbyAsync(line);
                }
                else if (_inputState == InputState.Room)
                {
                    await ProcessRoomAsync(line);
                }
            }
        }

        private async Task ProcessLobbyAsync(string line)
        {
            if (line.StartsWith('/'))
            {
                
            }
        }

        private async Task ProcessRoomAsync(string line)
        {
            
        }
    }
}

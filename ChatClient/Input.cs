using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                    await _session.DisconnectAsync((short)Packet.PacketType.CreateRoomReq, Encoding.UTF8.GetBytes("CLIENT INPUT EXIT"));
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
            if (line.StartsWith("//"))
            {
                string[] parts = line.Split(' ');

                if (string.Equals(parts[0], "//createRoom", StringComparison.OrdinalIgnoreCase))
                {
                    if (parts.Length > 1)
                    {
                        CreateRoomReq createRoomReq = new CreateRoomReq() { roomName = parts[1] };
                        await _session.SendAsync((short)PacketType.CreateRoomReq, PacketSerializer.Serialize(createRoomReq));
                    }
                    else
                    {
                        Console.WriteLine("방 생성 명령을 실행할 때, 방 이름을 추가로 입력하세요.");
                    }
                }
                else if (string.Equals(parts[0], "//roomList", StringComparison.OrdinalIgnoreCase))
                {
                    await _session.SendAsync((short)PacketType.RoomListReq, PacketSerializer.Serialize(new RoomListReq()));
                }
            }
            else
            {
                Console.WriteLine("로비에서는 명령어만 입력할 수 있습니다.");
            }
        }

        private async Task ProcessRoomAsync(string line)
        {
            if (line.StartsWith("//"))
            {

            }
        }
    }
}

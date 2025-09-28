using Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatClient
{
    internal class Input
    {
        private ServerSession _session;

        public Input(ServerSession session)
        {
            _session = session;
        }

        public async Task RunAsync()
        {
            while (false == _session.IsClosed)
            {
                string? line = await Console.In.ReadLineAsync();
                
                if (true == string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("//exit"))
                {
                    Console.WriteLine("채팅 클라이언트를 종료합니다.");

                    DisconnectReq disconnectReq = new DisconnectReq() { disconnectReason = 0 };
                    await _session.SendAsync((short)Packet.PacketType.DisconnectReq, PacketSerializer.Serialize(disconnectReq));
                }
                else if (ChatClient.ClientState == ClientState.Lobby)
                {
                    await ProcessLobbyAsync(line);
                }
                else if (ChatClient.ClientState == ClientState.Room)
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

                if (string.Equals(parts[0], "//roomCreate", StringComparison.OrdinalIgnoreCase))
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
                else if (string.Equals(parts[0], "//roomEnter", StringComparison.OrdinalIgnoreCase))
                {
                    if (parts.Length > 1)
                    {
                        uint roomId = 0;

                        if (uint.TryParse(parts[1], out roomId))
                        {
                            RoomEnterReq roomEnterReq = new RoomEnterReq() { roomId = roomId };
                            await _session.SendAsync((short)PacketType.RoomEnterReq, PacketSerializer.Serialize(roomEnterReq));
                        }
                        else
                        {
                            Console.WriteLine("적합한 형태의 방 번호를 입력하세요.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("방 입장 명령을 실행할 때, 방 번호를 추가로 입력하세요.");
                    }
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
            else
            {
                ChatReq chatReq = new ChatReq() { chatData = line };
                await _session.SendAsync((short)PacketType.ChatReq, PacketSerializer.Serialize(chatReq));
            }
        }
    }
}

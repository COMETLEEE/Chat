using System.Buffers.Binary;
using System.Net.Sockets;

namespace NetworkCore
{
    public abstract class Session
    {
        private TcpClient? _tcpClient;
        private NetworkStream? _networkStream;
        private RecvBuffer _recvBuffer;
        private int _isClosed = 0;

        private bool IsClosed => _isClosed != 0;

        public UniqueId<Session> SessionId { get; private set; }
        
        public Session(TcpClient tcpClient)
        {
            if (false == tcpClient.Connected)
            {
                throw new Exception("[Session] Constructor Exception - TcpClient is not connected.");
            }

            _tcpClient = tcpClient;
            _networkStream = tcpClient.GetStream();
            _recvBuffer = new RecvBuffer();
            _isClosed = 0;

            OnConnected();
        }

        public async Task RunAsync()
        {
            try
            {
                while (!IsClosed)
                {
                    int bytesRead = await _networkStream!.ReadAsync(_recvBuffer.WriteMemory);
                    if (bytesRead == 0)
                        break;

                    _recvBuffer.OnWrite(bytesRead);

                    while (_recvBuffer.DataSize >= 4)
                    {
                        short type = BinaryPrimitives.ReadInt16LittleEndian(_recvBuffer.ReadSpan.Slice(0, 2));
                        short length = BinaryPrimitives.ReadInt16LittleEndian(_recvBuffer.ReadSpan.Slice(2, 2));

                        // 패킷 한 덩어리가 아직 완전히 도착하지 않은 경우
                        // TCP 의 경우 스트림 순서가 보장되기 때문에 이후 도착할 데이터를 기다리기만 하면 됨
                        if (_recvBuffer.DataSize < length + 4)
                            break;

                        byte[] body = _recvBuffer.ReadSpan.Slice(4, length).ToArray();
                        await OnRecv((PacketType)type, body);

                        _recvBuffer.OnRead(length + 4);
                        _recvBuffer.Clear();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                // 네트워크 스트림이 닫혔을 때 발생할 수 있는 예외 무시
            }
            catch (Exception e)
            {
                // RunAsync 실행 중간 예외가 발생한 경우
                // E.X.) 다른 쓰레드에서 DisconnectAsync() 호출, ...
                Console.WriteLine($"[Session] RunAsync Exception: {e.ToString()}");
            }
            finally
            {
                _networkStream?.Close();
                _tcpClient?.Close();
                _isClosed = 1;

                OnDisconnected();
            }
        }

        public async Task SendAsync(PacketType type, byte[] body)
        {
            byte[] packet = new byte[4 + body.Length];

            BinaryPrimitives.WriteInt16LittleEndian(packet.AsSpan(0, 2), (short)type);
            BinaryPrimitives.WriteInt16LittleEndian(packet.AsSpan(2, 2), (short)body.Length);
            Array.Copy(body, 0, packet, 4, body.Length);

            try
            {
                await _networkStream!.WriteAsync(packet, 0, packet.Length);

                OnSend(packet.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Session] SendAsync Exception: {e.ToString()}");
            }
        }

        public async Task DisconnectAsync(PacketType type, byte[] body)
        {
            try
            {
                if (true == _tcpClient?.Connected)
                {
                    // 연결 종료 사유 패킷 전송
                    await SendAsync(type, body);

                    // 연결 상태 플래그 미리 Off
                    SetClosed();

                    // TCP 연결 닫기
                    try
                    {
                        _tcpClient?.Client.Shutdown(SocketShutdown.Both);
                    }
                    catch (SocketException)
                    {
                        // 이미 연결이 끊어진 경우 예외가 발생할 수 있으므로 무시
                    }

                    // 정리
                    _networkStream?.Close();
                    _tcpClient?.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Session] DisconnectAsync Exception: {e.ToString}");
            }
            finally
            {
                SetClosed();
                _networkStream = null;
                _tcpClient = null;
            }
        }

        protected abstract void OnConnected();
        protected abstract Task OnRecv(PacketType type, byte[] body);
        protected abstract void OnDisconnected();
        protected abstract void OnSend(int numOfBytes);

        private void SetClosed() => Interlocked.Exchange(ref _isClosed, 1);
    }
}

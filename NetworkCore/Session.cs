using System.Buffers.Binary;
using System.Net.Sockets;
using System.Threading.Channels;

namespace NetworkCore
{
    public abstract class Session
    {
        private UniqueId<Session> _sessionId = new UniqueId<Session>();
        private TcpClient? _tcpClient;
        private NetworkStream? _networkStream;
        private RecvBuffer _recvBuffer;
        private int _isClosed = 0;
        private Channel<(short, byte[])> _recvQueue = Channel.CreateUnbounded<(short, byte[])>();

        public bool IsClosed => _isClosed != 0;
        public uint SessionId => _sessionId.Id;

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
            // OnRecv() 콜백이 (콘텐츠 로직) RunAsync() 와 별도의 태스크에서 실행되도록
            _ = ProcessRecvQueueAsync();

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

                        // 패킷 한 묶음이 아직 완전히 도착하지 않은 경우
                        if (_recvBuffer.DataSize < length + 4)
                            break;

                        byte[] body = _recvBuffer.ReadSpan.Slice(4, length).ToArray();

                        await _recvQueue.Writer.WriteAsync((type, body));

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
                // RunAsync 실행 중간에 예외가 발생한 경우
                // E.X.) 다른 쓰레드에서 DisconnectAsync() 호출 (정상 종료), ...
                Console.WriteLine($"[Session] RunAsync Exception: {e.ToString()}");
            }
            finally
            {
                _networkStream?.Close();
                _tcpClient?.Close();
                _recvQueue.Writer.TryComplete();

                SetClosed();
                OnDisconnected();
            }
        }

        // 각 세션 별 논리적으로 패킷 전송 순서가 보장되어야 하므로
        // 외부에서 사용 시 대부분 'await' 가 필요하다.
        public async Task SendAsync(short type, byte[] body)
        {
            byte[] packet = new byte[4 + body.Length];

            BinaryPrimitives.WriteInt16LittleEndian(packet.AsSpan(0, 2), type);
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

        public void Disconnect()
        {
            try
            {
                if (true == _tcpClient?.Connected)
                {
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
                    _recvQueue.Writer.TryComplete();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Session] Disconnect Exception: {e.ToString}");
            }
            finally
            {
                SetClosed();
                _networkStream = null;
                _tcpClient = null;
            }
        }

        protected abstract void OnConnected();
        protected abstract Task OnRecv(short type, byte[] body);
        protected abstract void OnDisconnected();
        protected abstract void OnSend(int numOfBytes);

        private async Task ProcessRecvQueueAsync()
        {
            await foreach (var (type, body) in _recvQueue.Reader.ReadAllAsync())
            {
                await OnRecv(type, body);
            }
        }

        private void SetClosed() => Interlocked.Exchange(ref _isClosed, 1);
    }
}

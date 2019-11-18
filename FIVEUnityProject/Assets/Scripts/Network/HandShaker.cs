using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static FIVE.Network.NetworkUtil;

namespace FIVE.Network
{
    internal abstract class Handshaker
    {
        public abstract Task HandShakeAsync(TcpClient client, CancellationToken ct);
        public event Action<TcpClient> OnHandshakeSuccess;
        public event Action<TcpClient> OnHandshakeFail;
        public static Handshaker HostHandshaker { get; } = new HostSide();
        public static Handshaker ClientHandshaker { get; } = new ClientSide();
        private class HostSide : Handshaker
        {
            public override async Task HandShakeAsync(TcpClient client, CancellationToken ct)
            {
                NetworkStream stream = client.GetStream();
                byte[] headerBuffer = new byte[2];
                await stream.ReadAsync(headerBuffer, 0, 2, ct);
                if (!CheckHeader(headerBuffer))
                {
                    OnHandshakeFail?.Invoke(client);
                    return;
                }
                if (CheckPassword(stream))
                {
                    await stream.WriteAsync(((ushort)GameSyncHeader.AcceptJoin).ToBytes(), 0, 2, ct);
                    OnHandshakeSuccess?.Invoke(client);
                }
                else
                {
                    OnHandshakeFail?.Invoke(client);
                }
            }

            private static bool CheckPassword(NetworkStream stream)
            {
                if (!NetworkManager.Instance.RoomInfo.HasPassword)
                {
                    return true;
                }
                byte[] hashedPassword = stream.Read(16);
                return BytesCompare(hashedPassword, 0, NetworkManager.Instance.RoomInfo.HashedPassword, 0, 16);
            }

            private static unsafe bool CheckHeader(byte[] buffer)
            {
                fixed (byte* pBuffer = buffer)
                {
                    return *(ushort*)pBuffer == (ushort)GameSyncHeader.JoinRequest;
                }
            }
        }

        private class ClientSide : Handshaker
        {
            public override async Task HandShakeAsync(TcpClient client, CancellationToken ct)
            {
                NetworkStream stream = client.GetStream();
                await stream.WriteAsync(((ushort)GameSyncHeader.JoinRequest).ToBytes(), 0, 2, ct);
                if (NetworkManager.Instance.RoomInfo.HasPassword)
                {
                    await stream.WriteAsync(NetworkManager.Instance.RoomInfo.HashedPassword, 0, 16, ct);
                }
                if (CheckResult(stream))
                {
                    OnHandshakeSuccess?.Invoke(client);
                }
                else
                {
                    OnHandshakeFail?.Invoke(client);
                }
            }

            private unsafe bool CheckResult(NetworkStream stream)
            {
                byte[] buffer = stream.Read(2);
                fixed (byte* pBuffer = buffer)
                {
                    return *(ushort*)pBuffer == (ushort)GameSyncHeader.AcceptJoin;
                }
            }

        }
    }
}

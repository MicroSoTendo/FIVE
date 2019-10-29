using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using static FIVE.Network.NetworkUtil;

namespace FIVE.Network
{
    internal abstract class HandShaker
    {
        public abstract Task<int> HandShakeAsync(TcpClient client);

        public static HandShaker GetClientHandShaker(RoomInfo remoteRoomInfo)
        {
            return new ClientHandShaker(remoteRoomInfo);
        }

        public static HandShaker GetHostHandShaker(RoomInfo hostRoomInfo)
        {
            return new HostHandShaker(hostRoomInfo);
        }
        
        private class ClientHandShaker : HandShaker
        {
            private readonly RoomInfo remoteRoomInfo;
            public ClientHandShaker(RoomInfo remoteRoomInfo)
            {
                this.remoteRoomInfo = remoteRoomInfo;
            }
            public override async Task<int> HandShakeAsync(TcpClient client)
            {
                await client.ConnectAsync(new IPAddress(remoteRoomInfo.Host), remoteRoomInfo.Port);
                NetworkStream stream = client.GetStream();
                byte[] buffer;
                //Send join request
                if (remoteRoomInfo.HasPassword)
                {
                    buffer = new byte[4 + 16];
                    byte[] hash = remoteRoomInfo.HashedPassword;
                    buffer.CopyFrom(GameSyncCode.JoinRequest.ToBytes(), hash);
                    stream.Write(buffer);
                }
                else
                {
                    stream.Write(GameSyncCode.JoinRequest);
                }

                //Get response from server
                buffer = stream.Read(sizeof(int) * 2);
                var result = (GameSyncCode)buffer.ToI32();
                if (result.HasFlag(GameSyncCode.AcceptJoin))
                {
                    return buffer.ToI32(4);
                }
                return -1;
            }
        }


        private class HostHandShaker : HandShaker
        {
            private readonly RoomInfo hostRoomInfo;
            private readonly Random random;

            public HostHandShaker(RoomInfo hostRoomInfo)
            {
                this.hostRoomInfo = hostRoomInfo;
                random = new Random();
            }
            private int GenerateClientID()
            {
                return random.Next(0, int.MaxValue);
            }
            public override async Task<int> HandShakeAsync(TcpClient client)
            {
                NetworkStream stream = client.GetStream();
                bool hasPassword = hostRoomInfo.HasPassword;
                byte[] buffer = hasPassword ? new byte[4 + 16] : new byte[4];
                stream.Read(buffer);
                var code = (GameSyncCode)buffer.ToI32();
                if (code.HasFlag(GameSyncCode.JoinRequest))
                {
                    buffer = new byte[sizeof(int) * 2];
                    if (hasPassword)
                    {
                        if (!BytesCompare(buffer, 4, hostRoomInfo.HashedPassword, 0, 16))
                        {
                            Array.Clear(buffer, 0, buffer.Length);
                            stream.Write(buffer);
                            return -1;
                        }
                    }
                    int clientID = GenerateClientID();
                    buffer.CopyFromUnsafe(GameSyncCode.AcceptJoin.ToBytes(), clientID.ToBytes());
                    stream.Write(buffer);
                    return clientID;
                }
                return -1;
            }
        }
    }
}

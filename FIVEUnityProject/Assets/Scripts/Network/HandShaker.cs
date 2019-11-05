using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using static FIVE.Network.NetworkUtil;

namespace FIVE.Network
{
    internal abstract class HandShaker
    {
        public abstract Task<(int publicID, int privateID)> HandShakeAsync(TcpClient client);

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
            public override async Task<(int publicID, int privateID)> HandShakeAsync(TcpClient client)
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
                buffer = stream.Read(sizeof(int) * 3);
                var result = (GameSyncCode)buffer.ToI32();
                if (result.HasFlag(GameSyncCode.AcceptJoin))
                {
                    (int publicID, int privateID) = (buffer.ToI32(4), buffer.ToI32(8));
                }
                return (-1, -1);
            }
        }


        private class HostHandShaker : HandShaker
        {
            private readonly RoomInfo hostRoomInfo;
            private readonly Random random;
            private int NextPublicID = 1;
            public HostHandShaker(RoomInfo hostRoomInfo)
            {
                this.hostRoomInfo = hostRoomInfo;
                random = new Random();
            }
            private (int publicID, int privateID) GenerateClientID()
            {
                return (NextPublicID++, random.Next(0, int.MaxValue));
            }
            public unsafe override async Task<(int publicID, int privateID)> HandShakeAsync(TcpClient client)
            {
                NetworkStream stream = client.GetStream();
                bool hasPassword = hostRoomInfo.HasPassword;
                byte[] buffer = hasPassword ? new byte[4 + 16] : new byte[4];
                stream.Read(buffer);
                var code = (GameSyncCode)buffer.ToI32();
                if (code.HasFlag(GameSyncCode.JoinRequest))
                {
                    //Check if password is correct
                    if (hasPassword)
                    {
                        if (!BytesCompare(buffer, 4, hostRoomInfo.HashedPassword, 0, 16))
                        {
                            Array.Clear(buffer, 0, buffer.Length);
                            stream.Write(buffer);
                            return (-1, -1);
                        }
                    }

                    //Send back assgiend client ID
                    byte[] idBuffer = new byte[sizeof(int) * 3];
                    (int publicID, int privateID) = GenerateClientID();
                    fixed (byte* pBuffer = idBuffer)
                    {
                        *(int*)pBuffer = (int)GameSyncCode.AcceptJoin;
                        *((int*)pBuffer + 4) = publicID;
                        *((int*)pBuffer + 8) = privateID;
                    }
                    stream.Write(idBuffer);
                    return ( publicID,  privateID);
                }

                return (-1, -1);
            }
        }
    }
}

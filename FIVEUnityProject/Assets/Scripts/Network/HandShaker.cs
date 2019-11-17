using System;
using System.Collections.Generic;
using System.Linq;
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
                    buffer.CopyFrom(GameSyncHeader.JoinRequest.ToBytes(), hash);
                    stream.Write(buffer);
                }
                else
                {
                    stream.Write(GameSyncHeader.JoinRequest);
                }

                //Get response from server
                buffer = stream.Read(sizeof(int) * 3);
                var result = (GameSyncHeader)buffer.ToI32();
                if (result.HasFlag(GameSyncHeader.AcceptJoin))
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
            private readonly Dictionary<int, bool> actives;
            public HostHandShaker(RoomInfo hostRoomInfo)
            {
                this.hostRoomInfo = hostRoomInfo;
                random = new Random();
                actives = new Dictionary<int, bool>();
            }
            public unsafe override async Task<(int publicID, int privateID)> HandShakeAsync(TcpClient client)
            {
                NetworkStream stream = client.GetStream();
                bool hasPassword = hostRoomInfo.HasPassword;
                byte[] buffer = hasPassword ? new byte[4 + 16] : new byte[4];
                stream.Read(buffer);
                var code = (GameSyncHeader)buffer.ToI32();
                if (code.HasFlag(GameSyncHeader.JoinRequest))
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
                    int i = actives.FirstOrDefault(kvp => !kvp.Value).Key;
                    if (i == 0)
                    {
                        i = actives.Count;
                        actives.Add(i, true);
                    }
                    fixed (byte* pBuffer = idBuffer)
                    {
                        *(int*)pBuffer = (int)GameSyncHeader.AcceptJoin;
                        *(int*)(pBuffer + 4) = i;
                        *(int*)(pBuffer + 8) = random.Next(0, int.MaxValue);
                        stream.Write(idBuffer);
                        return (i, *(int*)(pBuffer + 8));
                    }
                }

                return (-1, -1);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace FIVE.Network
{
    internal class LobbyHandler
    {
        public bool IsReceivingLobbyInfo { get; set; }
        public int UpdateRate { get; set; } = 30;
        private IEnumerator ReceiveLobbyInfo()
        {
            while (IsReceivingLobbyInfo)
            {
                //while (!lobbyInfoClient.Connected)
                //{
                //    yield return new WaitForSeconds(1f / UpdateRate);
                //}

                //NetworkStream stream = lobbyInfoClient.GetStream();
                //stream.Write(0); //Reserved header
                //int count = stream.ReadI32();
                //roomInfos.Clear();
                //for (int i = 0; i < count; i++)
                //{
                //    byte[] roomInfoBuffer = new byte[stream.ReadI32()];
                //    stream.Read(roomInfoBuffer, 0, roomInfoBuffer.Length);
                //    var roomInfo = roomInfoBuffer.ToRoomInfo();
                //    roomInfos.TryAdd(roomInfo.Guid, roomInfo);
                //    yield return null;
                //}

                yield return new WaitForSeconds(15f / UpdateRate);
            }
        }

        //public static ICollection<RoomInfo> GetRoomInfos => roomInfos.Values;

        public static bool TryJoinRoom(Guid roomGuid, string password = "")
        {
            //if (roomInfos.TryGetValue(roomGuid, out RoomInfo roomInfo))
            //{
            //    gameClient = new TcpClient();
            //    gameClient.Connect(roomInfo.Host.ToString(), roomInfo.Port);
            //    return HandShakeToServer(gameClient, roomInfo.HasPassword, password);
            //}

            return false;
        }

        public static void CreateRoom(string name, int maxPlayers, ushort port = 8889, bool hasPassword = false,
            string password = "")
        {
            //hostRoomInfo.CurrentPlayers = 1; //Host self
            //hostRoomInfo.Name = name;
            //hostRoomInfo.MaxPlayers = maxPlayers;
            //hostRoomInfo.Port = port;
            //hostRoomInfo.HasPassword = hasPassword;
            //roomPassword = password;
            //if (hostInfoClient.Connected)
            //{
            //    NetworkStream stream = hostInfoClient.GetStream();
            //    stream.Write(OpCode.CreateRoom);
            //    stream.Write(hostRoomInfo);
            //    hostRoomInfo.Guid = stream.ReadGuid();
            //    gameServer = new TcpListener(IPAddress.Loopback, hostRoomInfo.Port);
            //    gameServer.Start();
            //    State = NetworkManager.NetworkState.Host;
            //    StartCoroutine(Host());
            //}
        }

        public static void RemoveRoom()
        {
            //if (hostInfoClient.Connected)
            //{
            //    NetworkStream stream = hostInfoClient.GetStream();
            //    stream.Write(OpCode.RemoveRoom);
            //    stream.Write(hostRoomInfo.Guid);
            //    gameServer.Stop();
            //    State = NetworkManager.NetworkState.Idle;
            //}
        }

        private static void UpdateRoomInfo(OpCode code)
        {
            //if (lobbyInfoClient.Connected)
            //{
            //    NetworkStream stream = hostInfoClient.GetStream();
            //    code |= OpCode.UpdateRoom;
            //    stream.Write(code);
            //    stream.Write(hostRoomInfo.Guid);
            //    switch (code)
            //    {
            //        case OpCode.UpdateName:
            //            stream.Write(hostRoomInfo.Name);
            //            break;
            //        case OpCode.UpdateCurrentPlayer:
            //            stream.Write(hostRoomInfo.CurrentPlayers);
            //            break;
            //        case OpCode.UpdateMaxPlayer:
            //            stream.Write(hostRoomInfo.MaxPlayers);
            //            break;
            //        case OpCode.UpdatePassword:
            //            stream.Write(hostRoomInfo.HasPassword);
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        public static void UpdateRoomName(string name)
        {
            //hostRoomInfo.Name = name;
            UpdateRoomInfo(OpCode.UpdateName | OpCode.UpdateRoom);
        }

        public static void UpdateCurrentPlayers(int current)
        {
            //hostRoomInfo.CurrentPlayers = current;
            UpdateRoomInfo(OpCode.UpdateCurrentPlayer | OpCode.UpdateRoom);
        }

        public static void UpdateMaxPlayers(int max)
        {
            //hostRoomInfo.MaxPlayers = max;
            UpdateRoomInfo(OpCode.UpdateMaxPlayer | OpCode.UpdateRoom);
        }

        public static void UpdatePassword(bool hasPassword, string password)
        {
            //hostRoomInfo.HasPassword = hasPassword;
            UpdateRoomInfo(OpCode.UpdatePassword | OpCode.UpdateRoom);
        }
    }
}

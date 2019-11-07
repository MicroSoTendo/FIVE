using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ListServerCore
{
    internal class Program
    {

        private static readonly HashSet<TcpClient> ConnectedClients = new HashSet<TcpClient>();
        private static readonly HashSet<Task> Tasks = new HashSet<Task>();
        private static void Main(string[] args)
        {
            IPAddress address = IPAddress.Any;
            int listenPort = 8888;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower() == "-p")
                {
                    if (i + 1 < args.Length && int.TryParse(args[i + 1], out int port))
                    {
                        if (port > 0 && port < ushort.MaxValue)
                        {
                            listenPort = port;
                        }
                    }
                }

                if (args[i].ToLower() == "-ip")
                {
                    if (i + 1 < args.Length)
                    {
                        if (IPAddress.TryParse(args[i + 1], out IPAddress parsedAddress))
                        {
                            address = parsedAddress;
                        }
                    }
                }
            }

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task handlerTask = Task.Run(() => { InComingHandler(address, listenPort); }, tokenSource.Token);
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                {
                    tokenSource.Cancel();
                }
                if (key.Key == ConsoleKey.R)
                {
                    handlerTask.Dispose();
                    foreach (Task task in Tasks)
                    {
                        task.Dispose();
                    }
                    foreach (TcpClient connectedClient in ConnectedClients)
                    {
                        connectedClient.Dispose();
                    }
                    ConnectedClients.Clear();
                    handlerTask = Task.Run(() => { InComingHandler(address, listenPort); }, tokenSource.Token);
                }
            }
        }

        private static readonly ConcurrentDictionary<Guid, RoomInfo> RoomInfos = new ConcurrentDictionary<Guid, RoomInfo>();
        private static readonly ConcurrentDictionary<TcpClient, Guid> HostDictionary = new ConcurrentDictionary<TcpClient, Guid>();
        private static unsafe void ClientHandler(TcpClient client)
        {
            try
            {
                NetworkStream networkStream = client.GetStream();
                byte[] opCodeBuffer = new byte[2];
                networkStream.Read(opCodeBuffer);
                fixed (byte* pBytes = opCodeBuffer)
                {
                    ushort* code = (ushort*)pBytes;
                    while (true)
                    {
                        if ((*code & (ushort)ListServerCode.CreateRoom) != 0)
                        {
                            Console.WriteLine(nameof(ListServerCode.CreateRoom) + "received");
                            CreateRoomHandler(client);
                        }
                        if ((*code & (ushort)ListServerCode.RemoveRoom) != 0)
                        {
                            Console.WriteLine(nameof(ListServerCode.RemoveRoom) + "received");
                            RemoveRoomHandler(client);
                        }
                        if ((*code & (ushort)ListServerCode.GetRoomInfos) != 0)
                        {
                            //Console.WriteLine(nameof(ListServerCode.GetRoomInfos) + "received");
                            SendRoomInfos(client);
                        }
                        if ((*code & (ushort)ListServerCode.UpdateRoom) != 0)
                        {

                        }

                    }
                }
            }
            catch
            {
                ExceptionHandle(client);
            }
        }

        private static void UpdateRoomHandler(ListServerCode code, NetworkStream networkStream)
        {
            byte[] guidBytes = new byte[16];
            networkStream.Read(guidBytes);
            Guid guid = guidBytes.ToGuid();
            switch (code)
            {
                case ListServerCode.UpdateName:
                    byte[] size = new byte[4];
                    networkStream.Read(size);
                    byte[] nameBuffer = new byte[size.ToI32()];
                    networkStream.Read(nameBuffer);
                    if (RoomInfos.ContainsKey(guid))
                    {
                        RoomInfo roomInfo = RoomInfos[guid];
                        roomInfo.Name = nameBuffer.ToName();
                        RoomInfos[guid] = roomInfo;
                    }
                    break;
                case ListServerCode.UpdateCurrentPlayer:
                    byte[] current = new byte[4];
                    networkStream.Read(current);
                    if (RoomInfos.ContainsKey(guid))
                    {
                        RoomInfo roomInfo = RoomInfos[guid];
                        roomInfo.CurrentPlayers = current.ToI32();
                        RoomInfos[guid] = roomInfo;
                    }
                    break;
                case ListServerCode.UpdateMaxPlayer:
                    byte[] max = new byte[4];
                    networkStream.Read(max);
                    if (RoomInfos.ContainsKey(guid))
                    {
                        RoomInfo roomInfo = RoomInfos[guid];
                        roomInfo.MaxPlayers = max.ToI32();
                        RoomInfos[guid] = roomInfo;
                    }
                    break;
                case ListServerCode.UpdatePassword:
                    byte[] flagBuffer = new byte[1];
                    networkStream.Read(flagBuffer);
                    if (RoomInfos.ContainsKey(guid))
                    {
                        RoomInfo roomInfo = RoomInfos[guid];
                        roomInfo.HasPassword = flagBuffer.ToBool();
                        RoomInfos[guid] = roomInfo;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void RemoveRoomHandler(TcpClient client)
        {
            var stream = client.GetStream();
            byte[] guidBytes = new byte[16];
            stream.Read(guidBytes);
            Guid guid = guidBytes.ToGuid();
            RoomInfos.TryRemove(guid, out _);
        }

        private static void CreateRoomHandler(TcpClient client)
        {
            IPAddress ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            Console.Write($"Create room requested from {ip}.");
            NetworkStream stream = client.GetStream();
            byte[] roomInfoBuffer = new byte[stream.ReadI32()];
            stream.Read(roomInfoBuffer);
            RoomInfo roomInfo = roomInfoBuffer.ToRoomInfo(ip.GetAddressBytes().ToI32());
            Console.WriteLine($"GUID = {roomInfo.Guid}, Room Name = {roomInfo.Name}, Max Players = {roomInfo.MaxPlayers}, Has Password = {roomInfo.HasPassword} ");
            stream.Write(roomInfo.Guid.ToBytes());
            RoomInfos.TryAdd(roomInfo.Guid, roomInfo);
            HostDictionary.TryAdd(client, roomInfo.Guid);
        }

        private static void SendRoomInfos(TcpClient client)
        {
            while (true)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(RoomInfos.Count.ToBytes());
                    foreach (RoomInfo roomInfo in RoomInfos.Values)
                    {
                        byte[] roomInfoBuffer = roomInfo.ToBytes();
                        stream.Write(roomInfoBuffer.Length.ToBytes());
                        stream.Write(roomInfoBuffer);
                    }
                }
                catch
                {
                    ExceptionHandle(client);
                    break;
                }
            }
        }

        private static void ExceptionHandle(TcpClient client)
        {
            ConnectedClients.Remove(client);
            if (HostDictionary.TryRemove(client, out Guid guid))
            {
                RoomInfos.TryRemove(guid, out _);
            }
        }

        private static void InComingHandler(IPAddress address, int port)
        {
            TcpListener listener = new TcpListener(address, port);
            listener.Start();
            Console.WriteLine($"Broadcasting at {address}:{port}");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ConnectedClients.Add(client);
                Console.WriteLine($"{(client.Client.RemoteEndPoint as IPEndPoint)?.Address} Connected ");
                Tasks.Add(Task.Run(() => ClientHandler(client)));
            }
        }
    }
}

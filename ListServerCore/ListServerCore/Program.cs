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

        private static bool TryParseArgs(IReadOnlyList<string> args,
            ref IPAddress address,
            ref int listeningPort,
            ref int broadcastPort)
        {
            bool ValidatePort(int p)
            {
                return p > 0 && p < 65535;
            }

            bool TryParsePort(string str, out int port)
            {
                if (int.TryParse(str, out port))
                {
                    if (ValidatePort(port))
                    {
                        return true;
                    }
                    Console.WriteLine($"Port {port} is in valid.");
                }
                else
                {
                    Console.WriteLine($"Port {str} is in valid.");
                }
                return false;
            }

            for (int i = 0; i < args.Count; i++)
            {
                switch (args[i])
                {
                    case "/p":
                    case "-p":
                        if (TryParsePort(args[i + 1], out listeningPort) && TryParsePort(args[i + 2], out broadcastPort))
                        {
                            i += 2;
                        }
                        return false;
                    case "-a":
                    case "/a":
                        if (IPAddress.TryParse(args[i + 1], out address))
                        {
                            i++;
                        }
                        else
                        {
                            Console.WriteLine($"Port {args[i + 1]} is in valid.");
                            return false;
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        private static readonly HashSet<TcpClient> ConnectedClients = new HashSet<TcpClient>();
        private static readonly HashSet<Task> Tasks = new HashSet<Task>();
        private static void Main(string[] args)
        {
            IPAddress address = IPAddress.Loopback;
            int updatePort = 8887;
            int infoPort = 8888;
            //if (!TryParseArgs(args, ref address, ref listeningPort, ref broadcastPort))
            //{
            //    return;
            //}
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task listenTask = Task.Run(() => { ListenUpdatePort(address, updatePort); }, tokenSource.Token);
            Task broadcastTask = Task.Run(() => { ListenInfoPort(address, infoPort); }, tokenSource.Token);
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                {
                    tokenSource.Cancel();
                }
                if (key.Key == ConsoleKey.R)
                {
                    listenTask.Dispose();
                    broadcastTask.Dispose();
                    foreach (Task task in Tasks)
                    {
                        task.Dispose();
                    }
                    foreach (TcpClient connectedClient in ConnectedClients)
                    {
                        connectedClient.Dispose();
                    }
                    ConnectedClients.Clear();
                    listenTask = Task.Run(() => { ListenUpdatePort(address, updatePort); }, tokenSource.Token);
                    broadcastTask = Task.Run(() => { ListenInfoPort(address, infoPort); }, tokenSource.Token);
                }
            }
        }

        private static readonly ConcurrentDictionary<Guid, RoomInfo> RoomInfos = new ConcurrentDictionary<Guid, RoomInfo>();
        private static void ClientHandler(TcpClient client)
        {
            try
            {
                NetworkStream networkStream = client.GetStream();
                byte[] opCodeBuffer = new byte[4];
                networkStream.Read(opCodeBuffer);
                int opCode = opCodeBuffer.ToI32();
                switch ((OpCode)opCode)
                {
                    case OpCode.CreateRoom:
                        CreateRoomHandler(networkStream, 0);
                        break;
                    case OpCode.RemoveRoom:
                        RemoveRoomHandler(networkStream);
                        break;
                    case OpCode.UpdateRoom:
                        UpdateRoomHandler(opCode, networkStream);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                // ignored
            }
        }

        private static void UpdateRoomHandler(int opCode, NetworkStream networkStream)
        {
            byte[] guidBytes = new byte[16];
            networkStream.Read(guidBytes);
            Guid guid = guidBytes.ToGuid();
            switch ((OpCode)opCode)
            {
                case OpCode.UpdateName:
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
                case OpCode.UpdateCurrentPlayer:
                    byte[] current = new byte[4];
                    networkStream.Read(current);
                    if (RoomInfos.ContainsKey(guid))
                    {
                        RoomInfo roomInfo = RoomInfos[guid];
                        roomInfo.CurrentPlayers = current.ToI32();
                        RoomInfos[guid] = roomInfo;
                    }
                    break;
                case OpCode.UpdateMaxPlayer:
                    byte[] max = new byte[4];
                    networkStream.Read(max);
                    if (RoomInfos.ContainsKey(guid))
                    {
                        RoomInfo roomInfo = RoomInfos[guid];
                        roomInfo.MaxPlayers = max.ToI32();
                        RoomInfos[guid] = roomInfo;
                    }
                    break;
                case OpCode.UpdatePassword:
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

        private static void RemoveRoomHandler(NetworkStream networkStream)
        {
            byte[] guidBytes = new byte[16];
            networkStream.Read(guidBytes);
            Guid guid = guidBytes.ToGuid();
            RoomInfos.TryRemove(guid, out _);
        }

        private static void CreateRoomHandler(NetworkStream stream, int ip)
        {
            Console.Write("Create room requested: ");
            byte[] sizeBuffer = new byte[4];
            stream.Read(sizeBuffer);
            byte[] roomInfoBuffer = new byte[sizeBuffer.ToI32()];
            stream.Read(roomInfoBuffer);
            RoomInfo roomInfo = roomInfoBuffer.ToRoomInfo();
            roomInfo.Host = ip;
            Console.WriteLine($"GUID = {roomInfo.Guid}, Room Name = {roomInfo.Name}, Max Players = {roomInfo.MaxPlayers}, Has Password = {roomInfo.HasPassword} ");
            stream.Write(roomInfo.Guid.ToBytes());
            RoomInfos.TryAdd(roomInfo.Guid, roomInfo);
        }

        private static void ListenUpdatePort(IPAddress address, int listeningPort)
        {
            TcpListener listener = new TcpListener(address, listeningPort);
            listener.Start();
            Console.WriteLine($"Listening at {address}:{listeningPort}");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ConnectedClients.Add(client);
                Console.WriteLine($"{(client.Client.RemoteEndPoint as IPEndPoint)?.Address} Connected ");
                Tasks.Add(Task.Run(() => ClientHandler(client)));
            }
        }

        private static void SendRoomInfos(TcpClient client)
        {
            NetworkStream networkStream = client.GetStream();
            while (true)
            {
                try
                {
                    byte[] head = new byte[4];
                    networkStream.Read(head);
                    networkStream.Write(RoomInfos.Values.Count.ToBytes());
                    foreach (RoomInfo roomInfo in RoomInfos.Values)
                    {
                        byte[] roomInfoBuffer = roomInfo.ToBytes();
                        networkStream.Write(roomInfoBuffer.Length.ToBytes());
                        networkStream.Write(roomInfoBuffer);
                    }
                }
                catch
                {
                    break;
                }
            }
        }

        private static void ListenInfoPort(IPAddress address, int broadcastPort)
        {
            TcpListener listener = new TcpListener(address, broadcastPort);
            listener.Start();
            Console.WriteLine($"Broadcasting at {address}:{broadcastPort}");
            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ConnectedClients.Add(client);
                Console.WriteLine($"{(client.Client.RemoteEndPoint as IPEndPoint)?.Address} Connected ");
                Tasks.Add(Task.Run(() => SendRoomInfos(client)));
            }
        }
    }
}

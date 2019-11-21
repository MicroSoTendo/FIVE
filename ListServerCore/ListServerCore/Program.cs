using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ListServerCore
{
    internal class Program
    {

        private static readonly HashSet<TcpClient> ConnectedClients = new HashSet<TcpClient>();
        private static readonly HashSet<Task> SendTasks = new HashSet<Task>();
        private static readonly HashSet<Task> ReadTasks = new HashSet<Task>();
        private static readonly HashSet<Task> TimerTasks = new HashSet<Task>();
        private static readonly ConcurrentDictionary<TcpClient, int> Timer = new ConcurrentDictionary<TcpClient, int>();
        private static readonly ConcurrentDictionary<TcpClient, ConcurrentQueue<ListServerHeader>> SendQueue 
            = new ConcurrentDictionary<TcpClient, ConcurrentQueue<ListServerHeader>>();
        private static readonly Dictionary<ListServerHeader, Action<TcpClient>> Handlers =
            new Dictionary<ListServerHeader, Action<TcpClient>>
            {
                {ListServerHeader.CreateRoom, CreateRoom },
                {ListServerHeader.AliveTick, AliveTick },
                {ListServerHeader.AssignGuid, AssignGuid },
                {ListServerHeader.RoomInfos, RoomInfos }
            };

        private static void AliveTick(TcpClient client)
        {
            Timer[client] = 0;
        }

        private static unsafe void RoomInfos(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[sizeof(ListServerHeader) + sizeof(int)];
            fixed (byte* pBuffer = buffer)
            {
                *(ListServerHeader*) pBuffer = ListServerHeader.RoomInfos;
                *(int*) (pBuffer + sizeof(ListServerHeader)) = RoomInfosByGuid.Count;
            }
            stream.Write(buffer);
            foreach (RoomInfo roomInfo in RoomInfosByGuid.Values)
            {
                byte[] roomInfoBuffer = roomInfo.ToBytes();
                stream.Write(roomInfoBuffer.Length.ToBytes());
                stream.Write(roomInfoBuffer);
            }
        }

        private static unsafe void AssignGuid(TcpClient client)
        {
            byte[] buffer = new byte[sizeof(ListServerHeader) + sizeof(Guid)];
            fixed (byte* pBuffer = buffer)
            {
                *(ListServerHeader*) pBuffer = ListServerHeader.AssignGuid;
                *(Guid*) (pBuffer + sizeof(ListServerHeader)) = GuidByClient[client];
            }
            client.GetStream().Write(buffer);
        }

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

            CancellationTokenSource cts = new CancellationTokenSource();
            Task handlerTask = HandleIncomingAsync(address, listenPort, cts.Token);
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q)
                {
                    cts.Cancel();
                }
                if (key.Key == ConsoleKey.R)
                {
                    handlerTask.Dispose();
                    foreach (Task task in SendTasks)
                    {
                        task.Dispose();
                    }
                    foreach (TcpClient connectedClient in ConnectedClients)
                    {
                        connectedClient.Dispose();
                    }
                    ConnectedClients.Clear();
                    handlerTask = HandleIncomingAsync(address, listenPort, cts.Token);
                }
            }
        }

        private static readonly ConcurrentDictionary<Guid, RoomInfo> RoomInfosByGuid = new ConcurrentDictionary<Guid, RoomInfo>();
        private static readonly ConcurrentDictionary<TcpClient, Guid> GuidByClient = new ConcurrentDictionary<TcpClient, Guid>();
        private static async Task ClientReadAsync(TcpClient client, CancellationToken ct)
        {
            NetworkStream networkStream = client.GetStream();
            byte[] headerBuffer = new byte[sizeof(ListServerHeader)];
            while (client.Connected)
            {
                await networkStream.ReadAsync(headerBuffer, 0, sizeof(ListServerHeader), ct);
                HandleListServerHeader(client, headerBuffer);
            }
        }

        private static async Task ClientWriteAsync(TcpClient client, CancellationToken ct)
        {
            ConcurrentQueue<ListServerHeader> queue = new ConcurrentQueue<ListServerHeader>();
            SendQueue.TryAdd(client, queue);
            while (!ct.IsCancellationRequested)
            {
                if (queue.TryDequeue(out ListServerHeader header))
                {
                    Handlers[header](client);
                }
                else
                {
                    Handlers[ListServerHeader.RoomInfos](client);
                }
                await Task.Delay(30, ct);
            }
        }

        private static async Task ClientTimeoutAsync(TcpClient client, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(1000, ct);
                Timer[client] += 1000;
                if (Timer[client] > 5000)
                {
                    RemoveRoomHandler(client);
                }
            }
        }

        private static unsafe void HandleListServerHeader(TcpClient client, byte[] headerBuffer)
        {
            fixed (byte* pBytes = headerBuffer)
            {
                ListServerHeader header = *(ListServerHeader*)pBytes;
                if (Handlers.TryGetValue(header, out Action<TcpClient> handler))
                {
                    handler(client);
                    AliveTick(client);
                }
            }
        }

        private static void UpdateRoomHandler(ListServerHeader header, NetworkStream networkStream)
        {
            byte[] guidBytes = new byte[16];
            networkStream.Read(guidBytes);
            Guid guid = guidBytes.ToGuid();
            switch (header)
            {
                case ListServerHeader.UpdateName:
                    byte[] size = new byte[4];
                    networkStream.Read(size);
                    byte[] nameBuffer = new byte[size.ToI32()];
                    networkStream.Read(nameBuffer);
                    if (RoomInfosByGuid.ContainsKey(guid))
                    {
                        RoomInfo roomInfo = RoomInfosByGuid[guid];
                        roomInfo.Name = nameBuffer.ToName();
                        RoomInfosByGuid[guid] = roomInfo;
                    }
                    break;
                case ListServerHeader.UpdateCurrentPlayer:
                    byte[] current = new byte[4];
                    networkStream.Read(current);
                    if (RoomInfosByGuid.ContainsKey(guid))
                    {
                        RoomInfo roomInfo = RoomInfosByGuid[guid];
                        roomInfo.CurrentPlayers = current.ToI32();
                        RoomInfosByGuid[guid] = roomInfo;
                    }
                    break;
                case ListServerHeader.UpdateMaxPlayer:
                    byte[] max = new byte[4];
                    networkStream.Read(max);
                    if (RoomInfosByGuid.ContainsKey(guid))
                    {
                        RoomInfo roomInfo = RoomInfosByGuid[guid];
                        roomInfo.MaxPlayers = max.ToI32();
                        RoomInfosByGuid[guid] = roomInfo;
                    }
                    break;
                case ListServerHeader.UpdatePassword:
                    byte[] flagBuffer = new byte[1];
                    networkStream.Read(flagBuffer);
                    if (RoomInfosByGuid.ContainsKey(guid))
                    {
                        RoomInfo roomInfo = RoomInfosByGuid[guid];
                        roomInfo.HasPassword = flagBuffer.ToBool();
                        RoomInfosByGuid[guid] = roomInfo;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void RemoveRoomHandler(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] guidBytes = new byte[16];
            stream.Read(guidBytes);
            Guid guid = guidBytes.ToGuid();
            RoomInfosByGuid.TryRemove(guid, out _);
        }

        private static unsafe void CreateRoom(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            IPAddress ip = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
            Console.Write($"Create room requested from {ip}.");

            int roomInfoSize = stream.Read<int>();
            byte[] roomInfoBuffer = new byte[roomInfoSize];
            stream.Read(roomInfoBuffer);
            fixed (byte* pBuffer = roomInfoBuffer)
            {
                RoomInfo roomInfo = new RoomInfo
                {
                    CurrentPlayers = *(int*)pBuffer,
                    MaxPlayers = *(int*)(pBuffer + 4),
                    HasPassword = *(bool*)(pBuffer + 8),
                    Port = *(ushort*)(pBuffer + 9),
                    Host = ip.GetAddressBytes().ToI32(),
                    Guid = Guid.NewGuid(),
                    Name = Encoding.Unicode.GetString(pBuffer + 11, roomInfoBuffer.Length - 11),
                };
                Console.WriteLine($"GUID = {roomInfo.Guid}, Room Name = {roomInfo.Name}, Max Players = {roomInfo.MaxPlayers}, Has Password = {roomInfo.HasPassword} ");
                RoomInfosByGuid.TryAdd(roomInfo.Guid, roomInfo);
                GuidByClient.TryAdd(client, roomInfo.Guid);
            }
            ScheduleSend(client, ListServerHeader.AssignGuid);
        }

        private static void ScheduleSend(TcpClient client, ListServerHeader code)
        {
            SendQueue[client].Enqueue(code);
        }

        private static void CleanUp(TcpClient client)
        {
            ConnectedClients.Remove(client);
            if (GuidByClient.TryRemove(client, out Guid guid))
            {
                RoomInfosByGuid.TryRemove(guid, out _);
            }
        }

        private static async Task HandleIncomingAsync(IPAddress address, int port, CancellationToken ct)
        {
            TcpListener listener = new TcpListener(address, port);
            listener.Start();
            Console.WriteLine($"Listening at {address}:{port}");
            while (!ct.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                ConnectedClients.Add(client);
                Console.WriteLine($"{(client.Client.RemoteEndPoint as IPEndPoint)?.Address} Connected ");
                SendTasks.Add(ClientWriteAsync(client, ct));
                ReadTasks.Add(ClientReadAsync(client, ct));
                TimerTasks.Add(ClientTimeoutAsync(client, ct));
            }
        }
    }
}

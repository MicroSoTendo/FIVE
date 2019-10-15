using Mirror;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using Telepathy;
using UnityEngine;

namespace FIVE.Network
{
    [RequireComponent(typeof(NetworkManager))]
    public class ListServer : MonoBehaviour
    {
        private readonly Client clientToListenConnection = new Client();

        private readonly Client gameServerToListenConnection = new Client();
        public ushort clientToListenPort = 8888;
        public string gameServerTitle = "Test Server";
        public ushort gameServerToListenPort = 8887;

        [Header("Listen Server Connection")] public string listServerIp = "gs.xugen.me";

        public Dictionary<string, ServerStatus> ServerList { get; } = new Dictionary<string, ServerStatus>();

        private void Start()
        {
            InvokeRepeating(nameof(Tick), 0, 1);
        }

        private bool IsConnecting()
        {
            return NetworkClient.active && !ClientScene.ready;
        }

        private bool FullyConnected()
        {
            return NetworkClient.active && ClientScene.ready;
        }

        // should we use the client to listen connection?
        private bool UseClientToListen()
        {
            return !NetworkManager.isHeadless && !NetworkServer.active && !FullyConnected();
        }

        // should we use the game server to listen connection?
        private bool UseGameServerToListen()
        {
            return NetworkServer.active;
        }

        private void Tick()
        {
            TickGameServer();
            TickClient();
        }

        // send server status to list server
        private void SendStatus()
        {
            var writer = new BinaryWriter(new MemoryStream());
            // create message
            writer.Write((ushort)NetworkServer.connections.Count);
            writer.Write((ushort)NetworkManager.singleton.maxConnections);
            byte[] titleBytes = Encoding.UTF8.GetBytes(gameServerTitle);
            writer.Write((ushort)titleBytes.Length);
            writer.Write(titleBytes);
            writer.Flush();
            
            // list server only allows up to 128 bytes per message
            if (writer.BaseStream.Position <= 128)
            {
                // send it
                gameServerToListenConnection.Send(((MemoryStream)writer.BaseStream).ToArray());
            }
            else
            {
                Debug.LogError(
                    "[List Server] List Server will reject messages longer than 128 bytes. Please use a shorter title.");
            }
        }

        private void TickGameServer()
        {
            // send server data to listen
            if (UseGameServerToListen())
            {
                // connected yet?
                if (gameServerToListenConnection.Connected)
                {
                    SendStatus();
                }
                // otherwise try to connect
                // (we may have just started the game)
                else if (!gameServerToListenConnection.Connecting)
                {
                    Debug.Log("[List Server] GameServer connecting......");
                    gameServerToListenConnection.Connect(listServerIp, gameServerToListenPort);
                }
            }
            // shouldn't use game server, but still using it?
            else if (gameServerToListenConnection.Connected)
            {
                gameServerToListenConnection.Disconnect();
            }
        }

        private void ParseMessage(byte[] bytes)
        {
            // note: we don't use ReadString here because the list server
            //       doesn't know C#'s '7-bit-length + utf8' encoding for strings
            var reader = new BinaryReader(new MemoryStream(bytes, false), Encoding.UTF8);

            byte ipBytesLength = reader.ReadByte();
            byte[] ipBytes = reader.ReadBytes(ipBytesLength);
            string ip = new IPAddress(ipBytes).ToString();
            ushort port = reader.ReadUInt16();
            ushort players = reader.ReadUInt16();
            ushort capacity = reader.ReadUInt16();
            ushort titleLength = reader.ReadUInt16();
            string title = Encoding.UTF8.GetString(reader.ReadBytes(titleLength));

            Debug.Log($"PARSED: ip= {ip}  port= {port}  players= {players} capacity= {capacity} title= {title}");
            // build key
            string key = ip + ":" + port;
            // find existing or create new one
            if (ServerList.TryGetValue(key, out ServerStatus server))
            {
                // refresh
                server.Title = title;
                server.Players = players;
                server.Capacity = capacity;
            }
            else
            {
                // create
                server = new ServerStatus(ip, port, title, players, capacity);
            }

            // save
            ServerList[key] = server;
        }

        private void TickClient()
        {
            // receive client data from listen
            if (UseClientToListen())
            {
                // connected yet?
                if (clientToListenConnection.Connected)
                {
                    // receive latest game server info
                    while (clientToListenConnection.GetNextMessage(out Message message))
                    {
                        // connected?
                        if (message.eventType == Telepathy.EventType.Connected)
                        {
                            Debug.Log("[List Server] Client connected!");
                        }
                        // data message?
                        else if (message.eventType == Telepathy.EventType.Data)
                        {
                            ParseMessage(message.data);
                        }
                        // disconnected?
                        else if (message.eventType == Telepathy.EventType.Connected)
                        {
                            Debug.Log("[List Server] Client disconnected.");
                        }
                    }

                    foreach (ServerStatus server in ServerList.Values)
                    {
                        if (server.Ping.isDone)
                        {
                            server.LastLatency = server.Ping.time;
                            server.Ping = new Ping(server.IP);
                        }
                    }
                }
                // otherwise try to connect
                // (we may have just joined the menu/disconnect from game server)
                else if (!clientToListenConnection.Connecting)
                {
                    Debug.Log("[List Server] Client connecting...");
                    clientToListenConnection.Connect(listServerIp, clientToListenPort);
                }
            }
            // shouldn't use client, but still using it? (e.g. after joining)
            else if (clientToListenConnection.Connected)
            {
                clientToListenConnection.Disconnect();
                ServerList.Clear();
            }
        }

        // disconnect everything when pressing Stop in the Editor
        private void OnApplicationQuit()
        {
            if (gameServerToListenConnection.Connected)
            {
                gameServerToListenConnection.Disconnect();
            }

            if (clientToListenConnection.Connected)
            {
                clientToListenConnection.Disconnect();
            }
        }
    }
}
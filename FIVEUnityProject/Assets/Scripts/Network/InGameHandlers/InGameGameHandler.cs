using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using static FIVE.Network.NetworkUtil;

namespace FIVE.Network.InGameHandlers
{
    internal abstract partial class InGameGameHandler : NetworkGameHandler
    {
        protected readonly TcpClient TcpClient;
        protected readonly NetworkStream Stream;
        protected InGameGameHandler(TcpClient client)
        {
            TcpClient = client;
            Stream = client.GetStream();
        }

        protected abstract void PreSync();
        protected abstract void OnSend();
        protected abstract void OnReceive();
        protected async Task Handler()
        {
            PreSync();
            Task sendTask = Send();
            Task receiveTask = Receive();
            await Task.WhenAll(sendTask, receiveTask);
        }

        private async Task Send()
        {
            while (TcpClient.Connected)
            {
                //GameSyncCode header = GameSyncCode.AliveTick;
                



                //await Stream.WriteAsync(header.ToBytes(), 0, 2);
            }
        }

        private async Task Receive()
        {
            while (TcpClient.Connected)
            {
                byte[] headerBuffer = new byte[2];
                await Stream.ReadAsync(headerBuffer, 0, 2);

            }
        }

    }
}
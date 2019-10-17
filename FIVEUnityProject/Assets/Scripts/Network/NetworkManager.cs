using System;
using UnityEngine;
using Apathy;

namespace FIVE.Network
{
    public class NetworkManager : MonoBehaviour
    {
        private string listServer;
        private ushort listServerPort;
        private Client listServerClient;
        public void Awake()
        {
            listServerClient = new Client();
            listServerClient.Connect(listServer, listServerPort);
        }
    }
}

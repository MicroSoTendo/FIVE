using System;
using UnityEngine;

namespace FIVE.Network.Core
{
    internal sealed class GameClient : MonoBehaviour
    {
        public long GameServerAddress { get; set; }
        public ushort GameServerPort { get; set; }

        private void Awake()
        {
            enabled = false;
        }
    }
}

using FIVE.Network;
using Mirror;
using UnityEngine;

namespace FIVE.GameStates
{
    public class MultiPlayer : GameMode
    {
        private NetworkManager networkManager;
        private ListServer listServer;
        private void Awake()
        {
            networkManager = gameObject.AddComponent<NetworkManager>();
            listServer = gameObject.AddComponent<ListServer>();
        }

        private void Start()
        {
        }

    }
}

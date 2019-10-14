using UnityEngine;

namespace FIVE.Network
{
    public class PlayerManager
    {
        [SerializeField] private readonly string PlayerNickName = "";
        private GameObject player;

        private void Start()
        {
            player.name = PlayerNickName;
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}
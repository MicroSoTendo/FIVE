using System;
using System.Collections;
using System.Collections.Generic;
using FIVE.EventSystem;
using FIVE.UI.StartupMenu;
using Photon.Pun;
using UnityEngine;


namespace FIVE.Network
{
    public class MultiplayersGame
    {
    
        public enum State
        {
            Host,
            Client,
        }

        public GameObject PlayerPrefab { get; set; }
        private State state;
        private event Action onUpdate;
        public MultiplayersGame(State state)
        {
            this.state = state;
            onUpdate += () => { };
            onUpdate += Start;
            Start();
        }
        
        private void Start()
        {
            if (state == State.Host)
            {
                // this.RaiseEvent<OnSinglePlayerButtonClicked>(EventArgs.Empty); //Fake it for now
                PhotonNetwork.LoadLevel("Multiplayers");
            }
            else
            {
                // var prefab = GameObject.FindObjectOfType<RobotManager>().RobotPrefab;
                // var go = PhotonNetwork.Instantiate(prefab.name, new Vector3(20f, 20f, 20f), Quaternion.identity);
            }

            onUpdate -= Start;
        }

        public IEnumerator UpdateCoroutine()
        {
            while (true)
            {
                onUpdate();
                //Debug.Log($"{nameof(PhotonNetwork.CountOfPlayers)} = {PhotonNetwork.CountOfPlayers}");
                yield return null;
            }
        }
    }
}

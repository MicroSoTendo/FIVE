using System.Collections.Generic;
using FIVE.Network.Views;
using Photon.Pun;
using UnityEngine;
using NetworkView = FIVE.Network.Views.NetworkView;


namespace FIVE.Network
{
    [RequireComponent(typeof(PhotonView))]
    public class MultiplayersManager : MonoBehaviourPun
    {
        public enum State
        {
            Host,
            Client,
        }

        public enum Observable
        {
            Transform,
            Animator,
            Rigidbody,
        }

        public Dictionary<GameObject, List<NetworkView>> NetworkedObjects { get; } 
            = new Dictionary<GameObject, List<NetworkView>>();
        private State state;
        private List<Component> PunObservables => photonView.ObservedComponents;
        void Awake()
        {
            state = PhotonNetwork.CurrentRoom.PlayerCount == 1
                ? State.Host
                : State.Client;
            photonView.ObservedComponents = new List<Component>();
        }

        public void NetworkThis(GameObject go, params Observable[] observables)
        {
            var punObservables = new List<NetworkView>();
            foreach (Observable observable in observables)
            {
                switch (observable)
                {
                    case Observable.Transform:
                        punObservables.Add(go.AddComponent<TransformView>());
                        break;
                    case Observable.Animator:
                        punObservables.Add(go.AddComponent<AnimatorView>());
                        break;
                    case Observable.Rigidbody:
                        punObservables.Add(go.AddComponent<RigidbodyView>());
                        break;
                    default:
                        break;
                }
            }
            NetworkedObjects.Add(go, punObservables);
            PunObservables.AddRange(punObservables);
        }
    }
}

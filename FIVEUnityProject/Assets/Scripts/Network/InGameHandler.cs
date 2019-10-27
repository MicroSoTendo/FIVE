using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using FIVE.Network.Serializers;
using UnityEngine;

namespace FIVE.Network
{
    public class InGameHandler
    {
        private IEnumerator InGameClientHandler(int id, TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            //Phase 1: send all existed objects
            GameObject[] networkedGameObjects = default;
            stream.Write(networkedGameObjects.Length);
            foreach (GameObject networkedGameObject in networkedGameObjects)
            {
                //stream.Write(SyncCenter.GetResourceID(networkedGameObject));
                //List<object> list = SyncCenter.GetSynchronizedComponent(networkedGameObject);
                List<object> list = default;
                stream.Write(list.Count);
                foreach (object o in list)
                {
                    switch (o)
                    {
                        case Transform t:
                            stream.Write(ComponentType.Transform);
                            Serializer.Get<Transform>().Serialize(t, out byte[] transformBytes);
                            stream.Write(transformBytes);
                            break;
                        case Animator animator:
                            break;
                    }
                }

                yield return null;
            }

            //Phase 2: do sync
            while (true)
            {
                //Get client state
                SyncHeader head = stream.Read<SyncHeader>();
                if (head != 0)
                {
                    if (head == SyncHeader.NetworkCall)
                    {
                        //HostResolveNetworkCall(id, stream);
                    }

                    if (head == SyncHeader.ComponentSync)
                    {
                        //HostResolveComponentSync(id, stream);
                    }
                }

                yield return null;
                //toBeSynced.Add((id, SyncCenter.GetClientObjects(id)));
                //Make sure collected all component sync and call
                //while (toBeSynced.Count < ConnectedClients.Count)
                //{
                //    yield return null;
                //}

                //Do passive sync broadcasting
                foreach ((int clientId, List<GameObject> gameObjects) in /*toBeSynced*/ new (int clientId, List<GameObject> gameObjects)[]{})
                {
                    if (clientId == id) //exclude ifself
                    {
                        continue;
                    }

                    foreach (GameObject go in gameObjects)
                    {
                        foreach (object o in new object[] { } /* SyncCenter.GetSynchronizedComponent(go)*/)
                        {
                            switch (o)
                            {
                                case Transform t:

                                    Serializer.Get<Transform>().Serialize(t, out byte[] transformBytes);
                                    stream.Write(ComponentType.Transform);
                                    stream.Write(transformBytes);
                                    break;
                                case Animator a:
                                    break;
                            }
                        }
                    }
                }

                yield return null;
            }
        }
        //Run by client
        private IEnumerator HostHandler(TcpClient gameClient)
        {
            NetworkStream stream = gameClient.GetStream();
            //Phase 1: fetch all existed objects
            int count = stream.ReadI32();
            for (int i = 0; i < count; i++)
            {
                int resourceID = stream.ReadI32();
                //GameObject go = Instantiate(SyncCenter.GetPrefab(resourceID));
                var go = new GameObject();
                int componentCount = stream.ReadI32();
                for (int j = 0; j < componentCount; j++)
                {
                    ComponentType componentType = stream.Read<ComponentType>();
                    switch (componentType)
                    {
                        case ComponentType.Transform:
                            byte[] transformBuffer = stream.Read(3 * 4 * 2);
                            Serializer.Get<Transform>().Deserialize(transformBuffer, go.transform);
                            break;
                        case ComponentType.Animator:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                yield return null;
            }

            //Phase 2: do sync
            while (true)
            {
                yield return null;
            }
        }
    }
}
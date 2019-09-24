using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.PrefabPool
{
    public class PrefabPools : IPunPrefabPool
    {

        public static PrefabPools Instance { get; } = new PrefabPools();
        private readonly DefaultPool defaultPool = new DefaultPool();
        private GameObject hackedGameObject;
        private bool isHacking;

        private PrefabPools()
        {
        }

        public void HackInstantiate(GameObject gameObject)
        {
            isHacking = true;
            hackedGameObject = gameObject;
            if (gameObject.GetComponent<PhotonView>() == null)
            {
                gameObject.AddComponent<PhotonView>();
            }
            PhotonNetwork.Instantiate("", Vector3.zero, Quaternion.identity);
        }

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            if (isHacking)
            {
                isHacking = false;
                return hackedGameObject;
            }
            return defaultPool.Instantiate(prefabId, position, rotation);
        }

        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
    }
}

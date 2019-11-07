using UnityEngine;

public class NPCInit : MonoBehaviour
{
    public static void Initialize()
    {
        GameObject NPC = Resources.Load<GameObject>("EntityPrefabs/NPC/NPCsinglePrefabs/NPCBlue");
        GameObject Spawner = Instantiate(NPC, new Vector3(-65.4f, 9.22f, 78.5f), Quaternion.Euler(new Vector3(0f, 140f, 0)));
        Spawner.transform.localScale = new Vector3(10, 10, 10);
        Spawner = Instantiate(NPC, new Vector3(68f, 9.41f, 58.4f), Quaternion.Euler(new Vector3(0f, 270f, 0)));
        Spawner.transform.localScale = new Vector3(10, 10, 10);
        Spawner = Instantiate(NPC, new Vector3(30.6f, 8.2f, 173.3f), Quaternion.Euler(new Vector3(0f, 180f, 0)));
        Spawner.transform.localScale = new Vector3(10, 10, 10);
        Spawner = Instantiate(NPC, new Vector3(-68.4f, 9.4f, 162.4f), Quaternion.Euler(new Vector3(0f, 140f, 0)));
        Spawner.transform.localScale = new Vector3(10, 10, 10);
        Spawner = Instantiate(NPC, new Vector3(65.9f, 9.1f, -148.1f), Quaternion.Euler(new Vector3(0f, 270f, 0)));
        Spawner.transform.localScale = new Vector3(10, 10, 10);
        Spawner = Instantiate(NPC, new Vector3(-54.2f, 10.1f, -153f), Quaternion.Euler(new Vector3(0f, 55f, 0)));
        Spawner.transform.localScale = new Vector3(10, 10, 10);
        GameObject NPCOrange = Resources.Load<GameObject>("EntityPrefabs/NPC/NPCsinglePrefabs/NPCOrange");
        Spawner = Instantiate(NPCOrange, new Vector3(16.7f, 0f, 65.3f), Quaternion.Euler(new Vector3(0f, -140f, 0)));
        Spawner.transform.localScale = new Vector3(10, 10, 10);
    }
}
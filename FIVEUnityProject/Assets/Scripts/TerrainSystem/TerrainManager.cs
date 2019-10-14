using FIVE.UI.StartupMenu;
using UnityEngine;
using static FIVE.Util;

namespace FIVE.TerrainSystem
{
    public class TerrainManager : MonoBehaviour
    {
        public GameObject Ground;

        //private GameObject Camera;
        //private readonly Dictionary<(int, int), AreaData> Areas = new Dictionary<(int, int), AreaData>();

        private void Awake()
        {
            //EventManager.Subscribe<OnSinglePlayerButtonClicked>((button, args) => { gameObject.SetActive(true); });
            //gameObject.SetActive(false);
            Subscribe<OnSinglePlayerButtonClicked>(OnSinglePlayerButtonClicked);
        }

        private void OnSinglePlayerButtonClicked()
        {
            Instantiate(Ground, Vector3.zero, Quaternion.identity);
        }

        private void Update()
        {
            //if (Camera == null)
            //{
            //    Camera = GameObject.FindGameObjectWithTag("MainCamera");
            //}

            //if (Camera != null)
            //{
            //    if (Time.frameCount % 60 == 0)
            //    {
            //        int x = (int)Camera.transform.position.x / AreaData.size;
            //        int y = (int)Camera.transform.position.z / AreaData.size;
            //        for (int i = -1; i <= 1; i++)
            //        {
            //            for (int j = -1; j <= 1; j++)
            //            {
            //                LoadArea(x + i, y + j);
            //            }
            //        }
            //    }
            //}
        }

        private void LoadArea(int x, int y)
        {
            //if (Areas.ContainsKey((x, y)))
            //{
            //    return;
            //}

            //var pos = new Vector3(x * AreaData.size, 0f, y * AreaData.size);

            //var ad = new AreaData(pos);
            //Areas[(x, y)] = ad;

            //GameObject o = Instantiate(Ground, pos, Quaternion.identity);
            //ad.ConstructArea(o);
        }
    }
}
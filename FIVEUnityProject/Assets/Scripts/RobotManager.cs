using FIVE.EventSystem;
using FIVE.UI.StartupMenu;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    public GameObject RobotPrefab;

    private void Awake()
    {
        EventManager.Subscribe<OnSinglePlayerButtonClicked>((b, args) => Instantiate(RobotPrefab, new Vector3(0f, 10f, 0f), Quaternion.identity));
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
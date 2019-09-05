using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSphere : MonoBehaviour
{
    enum ControllerState { FPS, RTS, };

    ControllerState currState = ControllerState.FPS;

    // Script References
    private RobotFreeAnim animator;
    private FpsController fpsController;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<RobotFreeAnim>();
        fpsController = new FpsController(GetComponent<CharacterController>());

        // Setup initial FPS Camera
        var eye = GameObject.Find("eyeDome");
        Camera.main.transform.parent = eye.transform;
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);
        Camera.main.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        fpsController.Update();
    }
}

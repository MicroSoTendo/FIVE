using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSphere : MonoBehaviour
{
    public enum RobotState { Idle, Walk, Jump, Open, };

    enum ControllerOp { FPS, RTS, };

    ControllerOp currOp = ControllerOp.FPS;
    public RobotState currState = RobotState.Idle;

    // Script References
    private RobotFreeAnim animator;
    private FpsController fpsController;

    void Start()
    {
        //animator = GetComponent<RobotFreeAnim>();
        animator = new RobotFreeAnim(gameObject);
        fpsController = new FpsController(GetComponent<CharacterController>(), gameObject);

        // Setup initial FPS Camera
        var eye = GameObject.Find("eyeDome");
        Camera.main.transform.parent = eye.transform;
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);
        Camera.main.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {
        animator.Update(currState);
        fpsController.Update();
    }
}

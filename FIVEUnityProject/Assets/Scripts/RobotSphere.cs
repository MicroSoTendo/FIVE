using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSphere : MonoBehaviour
{
    public enum RobotState { Idle, Walk, Jump, Open, };

    private enum ControllerOp
    { FPS, RTS, };

    private ControllerOp currOp = ControllerOp.FPS;
    public RobotState currState = RobotState.Idle;

    // Script References
    private RobotFreeAnim animator;

    private FpsController fpsController;

    private Camera fpsCamera;

    private void Awake()
    {
        // Setup initial FPS Camera
        fpsCamera = GameObject.Find("CameraManager").GetComponent<CameraManager>().NewCamera(gameObject.name + "Camera");
        var eye = GameObject.Find("eyeDome");
        fpsCamera.transform.parent = eye.transform;
        fpsCamera.transform.localPosition = new Vector3(0, 0, 0);
        fpsCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
        fpsCamera.tag = "MainCamera";
    }

    private void Start()
    {
        animator = new RobotFreeAnim(gameObject);
        fpsController = new FpsController(GetComponent<CharacterController>(), gameObject);
    }

    private void Update()
    {
        animator.Update(currState);
        fpsController.Update();
    }

    public void activateCamera()
    {
        fpsCamera.enabled = true;
    }
}
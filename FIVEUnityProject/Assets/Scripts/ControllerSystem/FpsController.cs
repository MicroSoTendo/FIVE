using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.ControllerSystem
{
    public class FpsController
{
    private GameObject gameObject;
    private CharacterController cc;
    private RobotSphere robotSphere;

    Vector3 rot = Vector3.zero;
    float rotSpeed = 40f;

    public FpsController(CharacterController cc, GameObject gameObject)
    {
        this.cc = cc;
        this.gameObject = gameObject;
        robotSphere = gameObject.GetComponent<RobotSphere>();
    }

    public void Update()
    {
        robotSphere.currState = RobotSphere.RobotState.Idle;
        if (Input.GetKey(KeyCode.W))
        {
            cc.SimpleMove(Vector3.forward);
            robotSphere.currState = RobotSphere.RobotState.Walk;
        }
        if (Input.GetKey(KeyCode.S))
        {
            cc.SimpleMove(-Vector3.forward);
            robotSphere.currState = RobotSphere.RobotState.Walk;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rot[1] -= rotSpeed * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rot[1] += rotSpeed * Time.fixedDeltaTime;
        }

        gameObject.transform.eulerAngles = rot;
    }
}

}

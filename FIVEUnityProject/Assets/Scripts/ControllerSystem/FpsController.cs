using UnityEngine;

namespace FIVE.ControllerSystem
{
    public class FpsController
    {
        private readonly GameObject gameObject;
        private readonly CharacterController cc;
        private readonly RobotSphere robotSphere;

        private Vector3 rot = Vector3.zero;
        private readonly float rotSpeed = 40f;

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
                cc.SimpleMove(gameObject.transform.TransformDirection(Vector3.forward));
                robotSphere.currState = RobotSphere.RobotState.Walk;
            }
            if (Input.GetKey(KeyCode.S))
            {
                cc.SimpleMove(gameObject.transform.TransformDirection(-Vector3.forward));
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
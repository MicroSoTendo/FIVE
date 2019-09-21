using UnityEngine;
using FIVE.Robot;

namespace FIVE.ControllerSystem
{
    [RequireComponent(typeof(Movable))]
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
            //robotSphere.currState = RobotSphere.RobotState.Idle;
            if (Input.GetKey(KeyCode.W))
            {
                //cc.SimpleMove(gameObject.transform.TransformDirection(Vector3.forward * 5));
                //robotSphere.currState = RobotSphere.RobotState.Walk;
                robotSphere.Move(Movable.Move.Front, 1);
            }
            if (Input.GetKey(KeyCode.S))
            {
                //cc.SimpleMove(gameObject.transform.TransformDirection(-Vector3.forward * 5));
                //robotSphere.currState = RobotSphere.RobotState.Walk;
                robotSphere.Move(Movable.Move.Back, 1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                //rot[1] -= rotSpeed * Time.fixedDeltaTime;
                robotSphere.Move(Movable.Move.Left, 1);
            }
            if (Input.GetKey(KeyCode.D))
            {
                //rot[1] += rotSpeed * Time.fixedDeltaTime;
                robotSphere.Move(Movable.Move.Right, 1);
            }

            gameObject.transform.eulerAngles = rot;
        }
    }
}
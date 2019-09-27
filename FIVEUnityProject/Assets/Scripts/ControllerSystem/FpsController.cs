using FIVE.Robot;
using UnityEngine;

namespace FIVE.ControllerSystem
{
    [RequireComponent(typeof(Movable))]
    public class FpsController
    {
        private readonly GameObject gameObject;
        private readonly CharacterController cc;
        private readonly RobotSphere robotSphere;

        private Vector3 rot = Vector3.zero;
        // private readonly float rotSpeed = 40f;

        public FpsController(CharacterController cc, GameObject gameObject)
        {
            this.cc = cc;
            this.gameObject = gameObject;
            robotSphere = gameObject.GetComponent<RobotSphere>();
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                robotSphere.Move(Movable.Move.Front, 1);
            }
            if (Input.GetKey(KeyCode.S))
            {
                robotSphere.Move(Movable.Move.Back, 1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                robotSphere.Move(Movable.Move.Left, 1);
            }
            if (Input.GetKey(KeyCode.D))
            {
                robotSphere.Move(Movable.Move.Right, 1);
            }
        }
    }
}
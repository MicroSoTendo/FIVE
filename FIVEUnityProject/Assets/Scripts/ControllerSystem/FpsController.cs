using FIVE.CameraSystem;
using FIVE.Robot;
using UnityEngine;

namespace FIVE.ControllerSystem
{
    [RequireComponent(typeof(Movable))]
    public class FpsController
    {
        private readonly CharacterController cc;
        private readonly GameObject gameObject;
        private readonly RobotSphere robotSphere;

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
                robotSphere.Move(Movable.Move.Left, 5);
            }

            if (Input.GetKey(KeyCode.D))
            {
                robotSphere.Move(Movable.Move.Right, 5);
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && CameraManager.CurrentActiveCamera.name.StartsWith("Robot POV"))
            {
                if (CameraManager.CurrentActiveCamera != null)
                {
                    Ray ray = CameraManager.CurrentActiveCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hitInfo))
                    {
                        if (hitInfo.collider.gameObject.name.StartsWith("AlienBeetle"))
                        {
                            robotSphere.Attack(hitInfo.collider.gameObject);
                        }
                        else
                        {
                            robotSphere.Attack(hitInfo.point);
                        }
                    }
                }
            }
        }
    }
}
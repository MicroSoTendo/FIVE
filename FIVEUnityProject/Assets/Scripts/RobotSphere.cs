using FIVE.ControllerSystem;
using FIVE.EventSystem;
using FIVE.EventSystem.EventTypes;
using UnityEngine;

namespace FIVE
{
    public class RobotSphere : MonoBehaviour
    {
        public enum RobotState { Idle, Walk, Jump, Open, };

        private enum ControllerOp { FPS, RTS, };

        private readonly ControllerOp currOp = ControllerOp.FPS;
        public RobotState currState = RobotState.Idle;

        public Camera CameraPrefab;

        // Script References
        private RobotFreeAnim animator;

        private FpsController fpsController;

        private Camera fpsCamera;

        private void Awake()
        {
            fpsCamera = Instantiate(CameraPrefab);
            Transform eye = transform.GetChild(0).GetChild(1); // HACK
            fpsCamera.transform.parent = eye;
            fpsCamera.transform.localPosition = new Vector3(0, 0, 0);
            fpsCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            Util.RaiseEvent<OnCameraCreated>(this, new OnCameraCreatedArgs { Id = "", Camera = fpsCamera });
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
    }
}
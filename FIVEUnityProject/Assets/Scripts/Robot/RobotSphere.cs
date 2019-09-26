using FIVE.AWSL;
using FIVE.CameraSystem;
using FIVE.ControllerSystem;
using FIVE.EventSystem;
using UnityEngine;

namespace FIVE.Robot
{
    [RequireComponent(typeof(Movable))]
    public class RobotSphere : MonoBehaviour
    {
        public enum RobotState { Idle, Walk, Jump, Open };

        private enum ControllerOp { FPS, RTS, };

        // private readonly ControllerOp currOp = ControllerOp.FPS;
        public RobotState currState = RobotState.Idle;

        private bool editingCode = false;
        private readonly LauncherEditorArgs code = new LauncherEditorArgs { Code = "" };

        private CharacterController cc;

        // Script References
        private RobotFreeAnim animator;
        private FpsController fpsController;
        private Camera fpsCamera;
        private Camera thirdPersonCamera;
        private Movable movable;

        private AwslScript script;
        public bool scriptActive;

        // Robot Status
        private float energy; // 1 - 100

        private void Awake()
        {

            GameObject eye = gameObject.GetChildGameObject(nameof(eye));
            fpsCamera = CameraManager.AddCamera(nameof(fpsCamera) + GetInstanceID(), eye.transform);
            fpsCamera.transform.localPosition = new Vector3(0, 0, 0);
            fpsCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);

            thirdPersonCamera = CameraManager.AddCamera(nameof(thirdPersonCamera) + GetInstanceID(), transform, true);
            thirdPersonCamera.transform.SetParent(transform);
            thirdPersonCamera.transform.localPosition = new Vector3(0, 2, 0);
            thirdPersonCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);

            cc = GetComponent<CharacterController>();
            movable = GetComponent<Movable>();

            // TODO: make networking non-invasive
            // if (photonView.IsMine == false && PhotonNetwork.IsConnected)
            // {
            //     fpsCamera.enabled = false;
            //     thirdPersonCamera.enabled = false;
            // }

            scriptActive = false;

            energy = 100f;
        }

        private void Start()
        {
            animator = new RobotFreeAnim(gameObject);
            fpsController = new FpsController(GetComponent<CharacterController>(), gameObject);
        }

        private void Update()
        {
            if (cc.velocity.magnitude == 0)
            {
                currState = RobotState.Idle;
            }
            else
            {
                currState = RobotState.Walk;
            }

            // TODO: Make network non-invasive
            // if (photonView.IsMine == false && PhotonNetwork.IsConnected)
            // {
            //     return;
            // }

            if (Input.GetKey(KeyCode.E))
            {
                editingCode = true;
                code.Saved = false;
                this.RaiseEvent<DoLaunchEditor, LauncherEditorArgs>(code);
                return;
            }

            if (editingCode)
            {
                if (code.Saved)
                {
                    editingCode = false;
                    script = new AwslScript(code.Code);
                    scriptActive = true;
                }
                return;
            }

            animator.Update(currState);
            if (scriptActive)
            {
                ExecuteScript();
            }
            else
            {
                fpsController.Update();
            }

            // energe loss
            energy -= Time.deltaTime;
            Debug.Log(energy);
        }

        public void LateUpdate()
        {
            // TODO: Make network non-invasive
            // if (photonView.IsMine == false && PhotonNetwork.IsConnected)
            // {
            //     return;
            // }
        }

        public void Move(Movable.Move move, int steps, bool schedule = false)
        {
            currState = RobotState.Walk;
            if (schedule)
            {
                movable.ScheduleMove(move, steps);
            }
            else
            {
                movable.MoveOnces[(int)move]();
            }
        }

        private void ExecuteScript()
        {
            script.Execute(gameObject);
        }
    }
}
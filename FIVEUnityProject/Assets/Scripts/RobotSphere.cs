using FIVE.ControllerSystem;
using FIVE.EventSystem;
using Photon.Pun;
using System;
using UnityEngine;

namespace FIVE
{
    [RequireComponent(typeof(PhotonView))]
    public class RobotSphere : MonoBehaviourPun
    {
        public enum RobotState { Idle, Walk, Jump, Open };

        private enum ControllerOp { FPS, RTS, };

        private readonly ControllerOp currOp = ControllerOp.FPS;
        public RobotState currState = RobotState.Idle;

        private bool editingCode = false;
        private readonly LauncherEditorArgs code = new LauncherEditorArgs { Code = "" };

        public Camera CameraPrefab;

        // Script References
        private RobotFreeAnim animator;

        private FpsController fpsController;

        private Camera fpsCamera;

        private AwslScript script;
        public bool scriptActive;

        private void Awake()
        {
            fpsCamera = Instantiate(CameraPrefab);
            Transform eye = transform.GetChild(0).GetChild(1); // HACK
            fpsCamera.transform.parent = eye;
            fpsCamera.transform.localPosition = new Vector3(0, 0, 0);
            fpsCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            this.RaiseEvent<OnCameraCreated>(new OnCameraCreatedArgs { Id = "Robot" + GetInstanceID(), Camera = fpsCamera });
            Camera camera2 = Instantiate(CameraPrefab);
            camera2.transform.parent = transform;
            camera2.transform.localPosition = new Vector3(0, 2, 0);
            camera2.transform.localRotation = Quaternion.Euler(90, 0, 0);
            this.RaiseEvent<OnCameraCreated>(new OnCameraCreatedArgs { Id = "Robot" + GetInstanceID() + " Camera 2", Camera = camera2 });
            this.RaiseEvent<OnLoadingGameMode>(EventArgs.Empty);
            if (photonView.IsMine == false && PhotonNetwork.IsConnected)
            {
                fpsCamera.enabled = false;
                camera2.enabled = false;
            }

            scriptActive = false;
        }

        private void Start()
        {
            animator = new RobotFreeAnim(gameObject);
            fpsController = new FpsController(GetComponent<CharacterController>(), gameObject);
        }

        private void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected)
            {
                return;
            }

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
        }

        public void LateUpdate()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected)
            {
                return;
            }
        }

        private void ExecuteScript()
        {
            script.Execute(gameObject);
        }
    }
}
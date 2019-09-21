using FIVE.ControllerSystem;
using FIVE.EventSystem;
using Photon.Pun;
using System;
using UnityEngine;
using FIVE.AWSL;
using FIVE.CameraSystem;

namespace FIVE.Robot
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(Movable))]
    public class RobotSphere : MonoBehaviourPun
    {
        public enum RobotState { Idle, Walk, Jump, Open };

        private enum ControllerOp { FPS, RTS, };

        private readonly ControllerOp currOp = ControllerOp.FPS;
        public RobotState currState = RobotState.Idle;

        private bool editingCode = false;
        private readonly LauncherEditorArgs code = new LauncherEditorArgs { Code = "" };

        // Script References
        private RobotFreeAnim animator;

        private FpsController fpsController;

        private Camera fpsCamera;
        private Camera thirdPersonCamera;
        private AwslScript script;
        public bool scriptActive;

        private void Awake()
        {
            
            GameObject eyeDome1 = gameObject.GetChildGameObject(nameof(eyeDome1));
            fpsCamera = CameraManager.AddCamera(nameof(fpsCamera), eyeDome1.transform);
            fpsCamera.transform.localPosition = new Vector3(0, 0, 0);
            fpsCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);

            thirdPersonCamera = CameraManager.AddCamera(nameof(thirdPersonCamera), transform);
            thirdPersonCamera.transform.SetParent(transform);
            thirdPersonCamera.transform.localPosition = new Vector3(0, 2, 0);
            thirdPersonCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);

            (fpsCamera.gameObject.GetComponentInChildren<AudioListener>() ?? fpsCamera.gameObject.GetComponent<AudioListener>()).enabled=false;
            (thirdPersonCamera.gameObject.GetComponentInChildren<AudioListener>() ?? thirdPersonCamera.GetComponent<AudioListener>()).enabled = true;
            if (photonView.IsMine == false && PhotonNetwork.IsConnected)
            {
                fpsCamera.enabled = false;
                thirdPersonCamera.enabled = false;
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
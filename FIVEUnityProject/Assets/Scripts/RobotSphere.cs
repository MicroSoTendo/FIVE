using FIVE.CameraSystem;
using FIVE.ControllerSystem;
using FIVE.EventSystem;
using Photon.Pun;
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
            //TODO: Check if camera is really "mine" in multiple players
            if (photonView.IsMine == false && PhotonNetwork.IsConnected) return;
            fpsCamera = Instantiate(CameraPrefab);
            Transform eye = transform.GetChild(0).GetChild(1); // HACK
            fpsCamera.transform.parent = eye;
            //fpsCamera.transform.position = eye.position;
            //fpsCamera.transform.rotation = eye.rotation;
            fpsCamera.transform.localPosition = new Vector3(0, 0, 0);
            fpsCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            this.RaiseEvent<OnCameraCreated>(new OnCameraCreatedArgs
            {
                Id = "Robot" + this.GetInstanceID(),
                Camera = fpsCamera
            });

            CameraManager.AddBinding(fpsCamera, eye);

            Camera camera2 = Instantiate(CameraPrefab);
            camera2.transform.parent = transform;
            camera2.transform.localPosition = new Vector3(0, 2, 0);
            camera2.transform.localRotation = Quaternion.Euler(90, 0, 0);
            this.RaiseEvent<OnCameraCreated>(new OnCameraCreatedArgs { Id = "Robot" + this.GetInstanceID() + " Camera 2", Camera = camera2 });



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
                this.RaiseEvent<DoLaunchEditor, LauncherEditorArgs>(code);
            }

            if (editingCode)
            {
                if (code.Saved)
                {
                    editingCode = false;
                    script = new AwslScript(code.Code);
                    scriptActive = true;
                }
            }
            else if (scriptActive)
            {
                animator.Update(currState);
                executeScript();
            }
            else
            {
                animator.Update(currState);
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

        private void executeScript()
        {
            script.execute(gameObject);
        }
    }
}
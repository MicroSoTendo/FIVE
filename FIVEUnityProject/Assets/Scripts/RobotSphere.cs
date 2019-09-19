using FIVE.ControllerSystem;
using FIVE.EventSystem;
using Photon.Pun;
using UnityEngine;

namespace FIVE
{
    public class RobotSphere : MonoBehaviour
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

<<<<<<< HEAD
        private PhotonView photonView;
=======
        private AwslScript script;
        private bool scriptActive;
>>>>>>> master

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            //TODO: Check if camera is really "mine" in multiple players
            fpsCamera = Instantiate(CameraPrefab);
            Transform eye = transform.GetChild(0).GetChild(1); // HACK
            fpsCamera.transform.parent = eye;
            fpsCamera.transform.localPosition = new Vector3(0, 0, 0);
            fpsCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            this.RaiseEvent<OnCameraCreated>(new OnCameraCreatedArgs { Id = "Robot" + this.GetInstanceID(), Camera = fpsCamera });

            Camera camera2 = Instantiate(CameraPrefab);
            camera2.transform.parent = transform;
            camera2.transform.localPosition = new Vector3(0, 2, 0);
            camera2.transform.localRotation = Quaternion.Euler(90, 0, 0);
<<<<<<< HEAD
            this.RaiseEvent<OnCameraCreated>(new OnCameraCreatedArgs { Id = "Robot" + this.GetInstanceID() + " Camera 2", Camera = camera2 });
=======
            Util.RaiseEvent<OnCameraCreated>(this, new OnCameraCreatedArgs { Id = "Robot" + this.GetInstanceID() + " Camera 2", Camera = camera2 });

            scriptActive = false;
>>>>>>> master
        }

        private void Start()
        {
            animator = new RobotFreeAnim(gameObject);
            fpsController = new FpsController(GetComponent<CharacterController>(), gameObject);
        }

        private void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
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
                }
            }
            else if (script != null)
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
<<<<<<< HEAD
        
=======

        private void executeScript()
        {
            script.execute(gameObject);
        }
>>>>>>> master
    }
}
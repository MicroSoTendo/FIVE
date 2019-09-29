using System;
using System.Collections;
using FIVE.AWSL;
using FIVE.CameraSystem;
using FIVE.ControllerSystem;
using FIVE.EventSystem;
using FIVE.UI;
using FIVE.UI.AWSLEditor;
using UnityEngine;

namespace FIVE.Robot
{
    [RequireComponent(typeof(Movable))]
    [RequireComponent(typeof(Battery))]
    [RequireComponent(typeof(Cpu))]
    public class RobotSphere : MonoBehaviour
    {
        public enum RobotState { Idle, Walk, Jump, Open };

        private enum ControllerOp { FPS, RTS, };

        // private readonly ControllerOp currOp = ControllerOp.FPS;
        public RobotState currState = RobotState.Idle;

        //private bool editingCode = false;
        //private readonly LauncherEditorArgs code = new LauncherEditorArgs { Code = "" };

        private CharacterController cc;

        // Robot Components
        public Battery battery;
        public Cpu cpu;

        // Script References
        private RobotFreeAnim animator;

        private FpsController fpsController;
        private Camera fpsCamera;
        private Camera thirdPersonCamera;
        private Movable movable;

        private AWSLScript script;
        private bool scriptActive;

        // Robot Status
        private float health;

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

            battery = GetComponent<Battery>();
            cpu = GetComponent<Cpu>();

            health = 100f;
            StartCoroutine(ToggleEditorCoroutine());
        }

        private void Start()
        {
            animator = new RobotFreeAnim(gameObject);
            fpsController = new FpsController(GetComponent<CharacterController>(), gameObject);
            EventManager.Subscribe<OnCodeEditorSaved, CodeEditorSavedEventArgs>(OnCodeSaved);
        }

        private void OnCodeSaved(object sender, CodeEditorSavedEventArgs e)
        {
            movable.ClearSchedule();
            script = new AWSLScript(this, e.Code);
            scriptActive = true;
        }

        private IEnumerator ToggleEditorCoroutine()
        {
            while (true)
            {
                if (!UIManager.GetViewModel<AWSLEditorViewModel>()?.IsFocused ?? true)
                {
                    if (Input.GetKey(KeyCode.E))
                    {
                        this.RaiseEventFixed<DoToggleEditor>(new LauncherEditorArgs(), 300);
                    }
                }
                yield return null;
            }
        }

        private void Update()
        {
            //Block user input when editor is up
            if (UIManager.GetViewModel<AWSLEditorViewModel>()?.IsEnabled ?? false)
            {
                return;
            }
            // update animation at beginning to ensure consistency
            animator.Update(currState);
            currState = cc.velocity.magnitude < float.Epsilon ? RobotState.Idle : RobotState.Walk;

            if (scriptActive)
            {
                ExecuteScript();
            }
            else
            {
                movable.ClearSchedule();
                fpsController.Update();
            }
            battery.Update();
            //Debug.Log(energy);
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
                movable.MoveOnces[(int)move](steps);
            }
        }

        private void ExecuteScript()
        {
            scriptActive = !script.Execute();
        }
    }
}
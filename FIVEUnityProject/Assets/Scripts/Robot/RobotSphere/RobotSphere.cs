using FIVE.AWSL;
using FIVE.CameraSystem;
using FIVE.ControllerSystem;
using FIVE.EventSystem;
using FIVE.RobotComponents;
using FIVE.UI;
using FIVE.UI.CodeEditor;
using System.Collections;
using UnityEngine;

namespace FIVE.Robot
{
    [RequireComponent(typeof(Movable))]
    [RequireComponent(typeof(CPU))]
    [RequireComponent(typeof(Battery))]
    public class RobotSphere : RobotBehaviour
    {
        public enum RobotSphereState { Idle, Walk, Jump, Open };

        private enum ControllerOp { FPS, RTS, };

        // private readonly ControllerOp currOp = ControllerOp.FPS;
        public RobotSphereState CurrentState = RobotSphereState.Idle;

        private CharacterController cc;

        // Robot Components
        public Battery Battery { get; private set; }

        public CPU CPU { get; private set; }

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

        protected override void Awake()
        {
            GameObject eye = gameObject.FindChildRecursive(nameof(eye));
            fpsCamera = CameraManager.AddCamera(nameof(fpsCamera) + GetInstanceID(), eye.transform);
            fpsCamera.transform.localPosition = new Vector3(0, 0, 0);
            fpsCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
            fpsCamera.gameObject.AddComponent<RobotCameraScanning>();

            thirdPersonCamera = CameraManager.AddCamera(nameof(thirdPersonCamera) + GetInstanceID(), transform, true);
            thirdPersonCamera.transform.SetParent(transform);
            thirdPersonCamera.transform.localPosition = new Vector3(0, 2, 0);
            thirdPersonCamera.transform.localRotation = Quaternion.Euler(90, 0, 0);

            cc = GetComponent<CharacterController>();
            movable = GetComponent<Movable>();

            scriptActive = false;

            Battery = GetComponent<Battery>();
            CPU = GetComponent<CPU>();

            health = 100f;
            StartCoroutine(ToggleEditorCoroutine());
            base.Awake();
        }

        protected override void Start()
        {
            animator = new RobotFreeAnim(gameObject);
            fpsController = new FpsController(GetComponent<CharacterController>(), gameObject);
            EventManager.Subscribe<OnCodeEditorSaved, CodeEditorSavedEventArgs>(OnCodeSaved);
            base.Start();
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
                if (!UIManager.Get<CodeEditorViewModel>()?.IsFocused ?? true)
                {
                    if (Input.GetKey(KeyCode.E))
                    {
                        this.RaiseEventFixed<OnToggleEditorRequested>(new LauncherEditorArgs(), 300);
                    }
                }
                yield return null;
            }
        }

        private void Update()
        {
            //Block user input when editor is up
            if (UIManager.Get<CodeEditorViewModel>()?.IsActive ?? false)
            {
                return;
            }
            // update animation at beginning to ensure consistency
            animator.Update(CurrentState);
            CurrentState = cc.velocity.magnitude < float.Epsilon ? RobotSphereState.Idle : RobotSphereState.Walk;

            if (scriptActive && movable.Moves.Count == 0)
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
        }

        public void Move(Movable.Move move, int steps, bool schedule = false)
        {
            CurrentState = RobotSphereState.Walk;
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
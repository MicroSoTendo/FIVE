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
        public int ID;

        public enum RobotSphereState { Idle, Walk, Jump, Open };

        public GameObject BulletPrefab;

        // Script References
        private RobotFreeAnim animator;

        private CharacterController cc;

        // private readonly ControllerOp currOp = ControllerOp.FPS;
        public RobotSphereState CurrentState = RobotSphereState.Idle;

        private Camera fpsCamera;

        private FpsController fpsController;

        // Robot Status
        private Movable movable => GetComponent<Movable>();

        private AWSLScript script;
        private bool scriptActive;
        private Camera thirdPersonCamera;

        // Robot Components
        public Battery Battery { get; private set; }

        public CPU CPU { get; private set; }

        protected override void Awake()
        {
            ID = RobotManager.NextID;

            cc = GetComponent<CharacterController>();

            scriptActive = false;

            Battery = GetComponent<Battery>();
            CPU = GetComponent<CPU>();

            StartCoroutine(ToggleEditorCoroutine());
            base.Awake();
        }

        protected override void Start()
        {
            GameObject eye = gameObject.FindChildRecursive(nameof(eye));
            fpsCamera = CameraManager.AddCamera(nameof(fpsCamera) + GetInstanceID(), parent: eye.transform);
            fpsCamera.gameObject.AddComponent<RobotCameraScanning>();
            thirdPersonCamera = CameraManager.AddCamera(nameof(thirdPersonCamera) + ID.ToString(),
                parent: transform, enableAudioListener: true,
                position: new Vector3(0, 2, 0),
                rotation: Quaternion.Euler(90, 0, 0));

            animator = new RobotFreeAnim(gameObject);
            fpsController = new FpsController(GetComponent<CharacterController>(), gameObject);
            EventManager.Subscribe<OnCodeEditorSaved, UpdateScriptEventArgs>(OnCodeSaved);
            OnLocalPlayerUpdate += RobotSphereUpdate;
            base.Start();
        }

        private void OnCodeSaved(object sender, UpdateScriptEventArgs e)
        {
            if (e.Target != this)
            {
                return;
            }

            if (movable.enabled)
            {
                movable.ClearSchedule();
            }

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
                        this.RaiseEventFixed<OnToggleEditorRequested>(new LauncherEditorArgs() { Target = this }, 300);
                    }
                }

                yield return null;
            }
        }

        private void RobotSphereUpdate()
        {
            //Block user input when editor is up
            if (UIManager.Get<CodeEditorViewModel>()?.IsActive ?? false)
            {
                return;
            }

            // update animation at beginning to ensure consistency
            animator.Update(CurrentState);
            CurrentState = cc.velocity.magnitude < float.Epsilon ? RobotSphereState.Idle : RobotSphereState.Walk;

            if (scriptActive && (!movable.enabled || movable.Moves.Count == 0))
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
            if (movable.enabled)
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
        }

        public void Attack(GameObject target)
        {
            Debug.Log("Attack");
            GameObject bullet = Instantiate(BulletPrefab, transform.position + transform.forward * 1.5f, Quaternion.identity);
            bullet.GetComponent<Bullet>().Target = target.transform.position;
        }

        private void ExecuteScript()
        {
            scriptActive = !script.Execute();
        }

        private enum ControllerOp { FPS, RTS, };
    }
}
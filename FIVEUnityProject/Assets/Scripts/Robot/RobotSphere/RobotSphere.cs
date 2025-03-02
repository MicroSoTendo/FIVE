﻿using FIVE.AWSL;
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
    [RequireComponent(typeof(AudioSource))]
    public class RobotSphere : RobotBehaviour
    {
        public int ID;

        public enum RobotSphereState { Idle, Walk, Jump, Open };

        public GameObject BulletPrefab;
        public GameObject GunfirePrefab;

        private AudioSource Shot;
        private AudioSource Walk;

        // Script References
        private RobotFreeAnim animator;

        private CharacterController cc;

        // private readonly ControllerOp currOp = ControllerOp.FPS;
        public RobotSphereState CurrentState = RobotSphereState.Idle;

        private Camera fpsCamera;

        private FpsController fpsController;

        // Robot Status
        public Movable movable;

        private AWSLScript script;
        public bool scriptActive;

        private Camera thirdPersonCamera;

        // Robot Components
        public Battery Battery { get; private set; }

        public CPU CPU { get; private set; }

        private float health;

        private GameObject flashlight;

        private float time;

        protected override void Awake()
        {
            ID = RobotManager.NextID;

            movable = GetComponent<Movable>();

            cc = GetComponent<CharacterController>();

            scriptActive = false;

            Battery = GetComponent<Battery>();
            CPU = GetComponent<CPU>();

            Shot = GetComponents<AudioSource>()[0];
            Walk = GetComponents<AudioSource>()[1];

            base.Awake();
        }

        protected override void Start()
        {
            fpsCamera = CameraManager.AddCamera("Robot POV " + ID.ToString(), parent: transform);
            fpsCamera.transform.localPosition = new Vector3(0f, 0.1f, 0.07f);
            fpsCamera.gameObject.AddComponent<RobotCameraScanning>();
            thirdPersonCamera = CameraManager.AddCamera("Robot " + ID.ToString(),
                parent: transform, enableAudioListener: true,
                position: new Vector3(0, 2, 0),
                rotation: Quaternion.Euler(90, 0, 0));

            GameObject light = transform.GetComponentInChildren<Light>().gameObject;
            light.SetParent(fpsCamera.transform);
            light.SetActive(false);
            flashlight = light;

            animator = new RobotFreeAnim(gameObject);
            fpsController = new FpsController(GetComponent<CharacterController>(), gameObject);
            EventManager.Subscribe<OnCodeEditorSaved, UpdateScriptEventArgs>(OnCodeSaved);
            OnFixedUpdate += RobotSphereUpdate;

            health = 100.0f;

            base.Start();
        }

        public void switchOnLight()
        {
            flashlight.SetActive(true);
        }

        private void OnMouseDown()
        {
            SwitchToThis();
        }

        public void SwitchToThis()
        {
            RobotManager.ActiveRobot = gameObject;
            CameraManager.SetCamera(fpsCamera);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe<OnCodeEditorSaved, UpdateScriptEventArgs>(OnCodeSaved);
        }

        private void OnCodeSaved(object sender, UpdateScriptEventArgs e)
        {
            if (movable.enabled)
            {
                movable.ClearSchedule();
            }
            else
            {
                movable.enabled = true;
            }

            if (e.Code.Trim().Length > 0)
            {
                script = new AWSLScript(this, e.Code);
                scriptActive = true;
            }
            else
            {
                movable.enabled = RobotManager.ActiveRobot == gameObject;
                scriptActive = false;
            }
        }

        private void RobotSphereUpdate()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                scriptActive = false;
                movable.enabled = true;
                movable.ClearSchedule();
            }

            //Block user input when editor is up
            if (UIManager.Get<CodeEditorViewModel>()?.IsActive ?? false)
            {
                return;
            }

            // update animation at beginning to ensure consistency
            animator.Update(CurrentState);
            CurrentState = cc.velocity.magnitude < float.Epsilon || GetComponent<Battery>().CurrentEnergy <= 0f ? RobotSphereState.Idle : RobotSphereState.Walk;

            if (scriptActive)
            {
                ExecuteScript();
            }
            else
            {
                fpsController.Update();
            }

            if (CurrentState == RobotSphereState.Walk)
            {
                time += Time.deltaTime;
                fpsCamera.transform.localPosition = new Vector3(Mathf.Sin(time * 8f) * 0.02f, 0.1f + Mathf.Sin(time * 16f) * 0.02f, 0.07f);
                if (!Walk.isPlaying)
                {
                    Walk.Play();
                }
            }
        }

        public void Move(Movable.Move move, int steps, bool schedule = false)
        {
            if (GetComponent<Battery>().CurrentEnergy > 0f && movable.enabled)
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

        private IEnumerator ShutGunfire(GameObject gunfire)
        {
            yield return new WaitForSeconds(0.1f);
            gunfire.SetActive(false);
            Destroy(gunfire);
        }

        private IEnumerator KillAlien(GameObject alien)
        {
            yield return new WaitForSeconds(0.2f);
            if (alien != null)
            {
                EnemyBehavior enemyBehavior = alien.GetComponent<EnemyBehavior>();
                if (enemyBehavior != null)
                {
                    enemyBehavior.OnHit();
                }
            }
        }

        // Attack on a target coordinate
        public void Attack(Vector3 target)
        {
            if (movable.enabled)
            {
                GameObject gunfire = Instantiate(GunfirePrefab, transform.position + transform.forward * 10f + new Vector3(0, 3, 0), Quaternion.identity);
                StartCoroutine(ShutGunfire(gunfire));
                GameObject bullet = Instantiate(BulletPrefab, transform.position + transform.forward * 10f + new Vector3(0, 1, 0), Quaternion.identity);
                bullet.GetComponent<Bullet>().Target = target;
                fpsCamera.GetComponent<CameraShake>().ShakeCamera(1.5f, 0.5f);
                Shot.Play();

                GetComponent<Battery>().CurrentEnergy -= 0.001f;
            }
        }

        // Attack on a GameObject (such as AlienBeetle)
        public void Attack(GameObject target)
        {
            if (movable.enabled)
            {
                GameObject gunfire = Instantiate(GunfirePrefab, transform.position + transform.forward * 10f + new Vector3(0, 3, 0), Quaternion.identity);
                StartCoroutine(ShutGunfire(gunfire));
                GameObject bullet = Instantiate(BulletPrefab, transform.position + transform.forward * 10f + new Vector3(0, 1, 0), Quaternion.identity);
                bullet.GetComponent<Bullet>().Target = target.transform.position;
                StartCoroutine(KillAlien(target));
                fpsCamera.GetComponent<CameraShake>().ShakeCamera(1.5f, 0.5f);
                Shot.Play();

                GetComponent<Battery>().CurrentEnergy -= 0.001f;
            }
        }

        public void OnHit()
        {
            health -= 5.0f;
            if (health <= 0)
            {
                if (RobotManager.ID2Robot.Count == 1)
                {
                    // lose
                }
                else
                {
                    RobotManager.RemoveRobot(gameObject);
                    CameraManager.Remove(fpsCamera);
                    CameraManager.Remove(thirdPersonCamera);

                    System.Collections.Generic.Dictionary<int, GameObject>.Enumerator it = RobotManager.ID2Robot.GetEnumerator();
                    it.MoveNext();
                    it.Current.Value.GetComponent<RobotSphere>().SwitchToThis();

                    gameObject.SetActive(false);
                    Destroy(gameObject);
                }
            }
        }

        private void ExecuteScript()
        {
            scriptActive = !script.Execute();
            if (!scriptActive)
            {
                movable.enabled = RobotManager.ActiveRobot == gameObject;
            }
        }
    }
}
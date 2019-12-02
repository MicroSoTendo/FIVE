using FIVE.Robot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    [RequireComponent(typeof(CharacterController))]
    public class EnemyBehavior : MonoBehaviour
    {
        public static List<GameObject> DropPickups;

        public GameObject BulletPrefab;
        public GameObject GunfirePrefab;

        private AudioSource Shot;

        private enum State { Idle, Walk, Attack, };

        private State state;
        private Animator animator;

        private Material red;
        private Material bug;
        private CharacterController cc;
        private Renderer rdr;

        private GameObject currTarget;

        private float patrolDirection;

        private float visionRange;
        private float attackRange;
        private float speed;
        private float elapsedTime;
        private float health;

        private void Awake()
        {
            if (DropPickups == null)
            {
                DropPickups = new List<GameObject>
                {
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Capacitor_Ceramic_Blue"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Capacitor_Electrolytic"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Capacitor_MemoryBackup"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Capacitor_Polypropylene"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Diode_LightEmitting"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Diode_Schottky"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Diode_Zener"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/inductor_Air"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/inductor_Ferrite"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Inductor_Ferrite_Toroidal"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Resistor_Big"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Resistor_Medium"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Resistor_Small"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Transistor_TO92"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Transistor_TO126"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Transistor_TO220"),
                    Resources.Load<GameObject>("EntityPrefabs/ElectronicPrefabs/Presentation/Wire")
                };
                Debug.Assert(DropPickups.TrueForAll(o => o != null));
            }

            Shot = GetComponents<AudioSource>()[0];
        }

        private void Start()
        {
            visionRange = 50.0f;

            red = Resources.Load<Material>("Materials/Red");
            bug = Resources.Load<Material>("Materials/AlienBeetle/skin1");
            if (red == null)
            {
                Debug.Log("Red not found");
            }
            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            rdr = GetComponentInChildren<SkinnedMeshRenderer>();
            if (rdr == null)
            {
                Debug.Log("Renderer not found");
            }

            patrolDirection = Random.Range(0, 360);
            transform.Rotate(0, patrolDirection, 0);
            state = State.Idle;

            attackRange = cc.radius + 3.0f;
            speed = 20.0f;
            elapsedTime = 0;
            health = 100.0f;
        }

        private void FixedUpdate()
        {
            elapsedTime += Time.deltaTime;
            if (state == State.Idle)
            {
                animator.SetTrigger("idle2");
            }
            else if (state == State.Walk)
            {
                animator.SetTrigger("walk");
            }
            else
            {
                animator.SetTrigger("attack1");
                StartCoroutine(AttackDelay());
            }

            if (currTarget == null)
            {
                Patrol();
                SearchTarget();
            }
            else
            {
                float distance = Vector3.Distance(transform.position, currTarget.transform.position);
                if (distance > 2 * visionRange)
                {
                    state = State.Idle;
                    currTarget = null;
                }
                else if (distance < visionRange)
                {
                    state = State.Idle;
                    if (elapsedTime > 0.5)
                    {
                        Attack(currTarget);
                    }
                }
                else
                {
                    state = State.Walk;
                    Vector3 targetDirection = currTarget.transform.position - transform.position;
                    float singleStep = 1.0f * Time.deltaTime;
                    var newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

                    transform.rotation = Quaternion.LookRotation(newDirection);

                    cc.SimpleMove(Vector3.Normalize(newDirection) * speed * Time.deltaTime);
                }
            }
            if (elapsedTime > 0.5)
            {
                elapsedTime = 0;
            }
        }

        private IEnumerator ShutGunfire(GameObject gunfire)
        {
            yield return new WaitForSeconds(0.1f);
            gunfire.SetActive(false);
            Destroy(gunfire);
        }

        private IEnumerator AttackPlayer(GameObject player)
        {
            yield return new WaitForSeconds(0.2f);
            if (player != null)
            {
                RobotSphere enemyBehavior = player.GetComponent<RobotSphere>();
                if (enemyBehavior != null)
                {
                    enemyBehavior.OnHit();
                }
            }
        }

        public void Attack(GameObject target)
        {
            GameObject gunfire = Instantiate(GunfirePrefab, transform.position + transform.forward * 10f + new Vector3(0, 3, 0), Quaternion.identity);
            StartCoroutine(ShutGunfire(gunfire));
            GameObject bullet = Instantiate(BulletPrefab, transform.position + transform.forward * 10f + new Vector3(0, 1, 0), Quaternion.identity);
            bullet.GetComponent<Bullet>().Target = target.transform.position;
            if (Random.value < 1.0)
            {
                StartCoroutine(AttackPlayer(target));
            }
            Shot.Play();
        }

        public void OnHit()
        {
            health -= 5.0f;
            FlashRed();
            GetComponent<AudioSource>().Play();
            if (health <= 0.0f)
            {
                Enemy.EnemyManager.Remove(gameObject);
                gameObject.SetActive(false);
                GenPickups();
                Destroy(gameObject);
            }
        }

        private void SearchTarget()
        {
            GameObject nearPlayer = null;
            float nearDist = 1000;
            foreach (GameObject robot in RobotManager.Robots)
            {
                float distance = Vector3.Distance(robot.transform.position, transform.position);
                if (distance < nearDist)
                {
                    nearDist = distance;
                    nearPlayer = robot;
                }
            }

            if (nearPlayer != null)
            {
                currTarget = nearPlayer;
            }
        }

        private void Patrol()
        {
            state = State.Walk;
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 3)
            {
                patrolDirection = Random.Range(0, 90);
                transform.Rotate(0, patrolDirection, 0);
                elapsedTime = 0;
            }
            else
            {
                cc.SimpleMove(transform.forward * speed);
            }
        }

        private IEnumerator AttackDelay()
        {
            yield return new WaitForSeconds(1f);

            if (currTarget != null)
            {
                currTarget.GetComponent<RobotSphere>().OnHit();
                if (!currTarget.activeSelf)
                {
                    currTarget = null;
                }
            }
        }

        private void FlashRed()
        {
            rdr.material = red;
            Invoke("ResetColor", 0.5f);
        }

        private void ResetColor()
        {
            rdr.material = bug;
        }

        private void GenPickups()
        {
            int index = Random.Range(0, DropPickups.Count);
            GameObject p = Instantiate(DropPickups[index], transform.position, Quaternion.identity);
            Debug.Log(p.name);
            p.transform.localScale = new Vector3(80, 80, 80);
        }
    }
}
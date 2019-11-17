using FIVE.Robot;
using UnityEngine;

namespace FIVE
{
    [RequireComponent(typeof(CharacterController))]
    public class EnemyBehavior : MonoBehaviour
    {
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
        private float speed;
        private float elapsedTime;
        private float health;

        private void Start()
        {
            visionRange = 10.0f;

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
            
            speed = 10.0f;
            elapsedTime = 0;
            health = 100.0f;
        }

        private void Update()
        {
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
                else if (distance < 6.0f)
                {
                    state = State.Attack;
                }
                else
                {
                    state = State.Walk;
                    Vector3 direction = currTarget.transform.position - transform.position;
                    transform.forward = direction;
                    if (direction.magnitude > 5.0f)
                    {
                        cc.SimpleMove(Vector3.Normalize(direction) * speed);
                    }
                }
            }
        }

        public void OnHit()
        {
            health -= 30.0f;
            FlashRed();
            if (health <= 0.0f)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }

        private void SearchTarget()
        {
            foreach (GameObject robot in RobotManager.Robots)
            {
                if (Physics.SphereCast(transform.position, 3.0f, robot.transform.position - transform.position,
                    out RaycastHit hitInfo, visionRange))
                {
                    currTarget = robot;
                }
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
        
        private void FlashRed()
        {
            rdr.material = red;
            Invoke("ResetColor", 0.5f);
        }

        private void ResetColor()
        {
            rdr.material = bug;
        }
    }
}
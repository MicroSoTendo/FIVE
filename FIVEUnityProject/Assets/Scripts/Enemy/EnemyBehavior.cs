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

        private CharacterController cc;

        private GameObject currTarget;

        private float patrolDirection;

        private float visionRange;

        private float speed;
        private float elapsedTime;

        private void Start()
        {
            visionRange = 10.0f;

            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            patrolDirection = Random.Range(0, 360);
            transform.Rotate(0, patrolDirection, 0);
            state = State.Idle;

            speed = 10.0f;
            elapsedTime = 0;
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
                Debug.Log("Attacking");
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
                Debug.Log(distance);
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

        private void SearchTarget()
        {
            foreach (GameObject robot in RobotManager.Instance.Robots)
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
    }
}
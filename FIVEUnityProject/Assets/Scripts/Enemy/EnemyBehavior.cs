using FIVE.Robot;
using UnityEngine;

namespace FIVE
{
    [RequireComponent(typeof(CharacterController))]
    public class EnemyBehavior : MonoBehaviour
    {
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

            speed = 10.0f;
            elapsedTime = 0;
        }

        private void Update()
        {
            if (cc.velocity.magnitude > 0)
            {
                animator.SetTrigger("walk");
            }
            else
            {
                animator.SetTrigger("idle2");
            }

            if (currTarget == null)
            {
                Patrol();
                SearchTarget();
            }
            else
            {
                if (Vector3.Distance(transform.position, currTarget.transform.position) > 2 * visionRange)
                {
                    currTarget = null;
                }
                else
                {
                    Vector3 distance = currTarget.transform.position - transform.position;
                    transform.forward = distance;
                    if (distance.magnitude > 5.0f)
                    {
                        cc.SimpleMove(Vector3.Normalize(distance) * speed);
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
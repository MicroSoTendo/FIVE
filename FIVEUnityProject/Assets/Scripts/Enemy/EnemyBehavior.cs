using FIVE.Robot;
using UnityEngine;

namespace FIVE
{
    [RequireComponent(typeof(CharacterController))]
    public class EnemyBehavior : MonoBehaviour
    {
        public float VisionRange;

        private CharacterController cc;
        private Animator animator;

        private GameObject currTarget;

        private float speed;

        private void Start()
        {
            VisionRange = 10.0f;

            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            speed = 10.0f;
        }

        private void Update()
        {
            if (currTarget == null || Vector3.Distance(transform.position, currTarget.transform.position) > 2 * VisionRange)
            {
                animator.SetTrigger("idle2");
                SearchTarget();
            }
            else
            {
                animator.SetTrigger("walk");
                Vector3 distance = currTarget.transform.position - transform.position;
                transform.forward = distance;
                if (distance.magnitude > 5.0f)
                {
                    cc.SimpleMove(Vector3.Normalize(distance) * speed);
                }
            }
        }

        private void SearchTarget()
        {
            foreach (GameObject robot in RobotManager.Instance.Robots)
            {
                if (Physics.SphereCast(transform.position, 3.0f, robot.transform.position - transform.position, out RaycastHit hitInfo, VisionRange))
                {
                    currTarget = robot;
                }
            }
        }

        private void Patrol()
        {
        }
    }
}
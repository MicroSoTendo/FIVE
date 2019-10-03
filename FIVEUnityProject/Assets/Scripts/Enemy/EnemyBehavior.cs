using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FIVE.Robot;

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

        void Start()
        {
            VisionRange = 10.0f;

            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            speed = 10.0f;
        }

        void Update()
        {
            if (currTarget == null || Vector3.Distance(transform.position, currTarget.transform.position) > 2 * VisionRange)
            {
                animator.SetTrigger("idle2");
                SearchTarget();
            }
            else
            {
                animator.SetTrigger("walk");
                Vector3 move = currTarget.transform.position - transform.position;
                transform.forward = move;
                cc.SimpleMove(Vector3.Normalize(move) * speed);
            }
        }

        private void SearchTarget()
        {
            foreach (GameObject robot in RobotManager.Instance().Robots)
            {
                if (Physics.SphereCast(transform.position, 3.0f, robot.transform.position - transform.position, out RaycastHit hitInfo, VisionRange))
                {
                    currTarget = robot;
                }
            }
        }
    }
}

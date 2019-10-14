using UnityEngine;

namespace FIVE.Robot
{
    public class RobotFreeAnim
    {
        private readonly Animator anim;
        private readonly GameObject gameObject;

        public RobotFreeAnim(GameObject gameObject)
        {
            this.gameObject = gameObject;
            anim = gameObject.GetComponent<Animator>();
        }

        public void Update(RobotSphere.RobotSphereState currSphereState)
        {
            UpdateAnim(currSphereState);
        }

        private void UpdateAnim(RobotSphere.RobotSphereState currSphereState)
        {
            if (currSphereState == RobotSphere.RobotSphereState.Idle)
            {
                anim.SetBool("Walk_Anim", false);
            }
            else if (currSphereState == RobotSphere.RobotSphereState.Walk)
            {
                anim.SetBool("Walk_Anim", true);
            }
        }
    }
}
using UnityEngine;

namespace FIVE.ControllerSystem
{
    public class RtsControlUnit : MonoBehaviour
    {
        private CharacterController cc;

        private Vector3 currTarget;
        private Vector3 velocity;
        // private readonly float speed = 10f;

        private void Start()
        {
            cc = gameObject.GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (Vector3.Distance(currTarget, transform.position) > 0.1f && cc)
            {
                cc.SimpleMove(velocity);
            }
        }

        public void OnSelect()
        {
            //ObjectRenderer.material = ClickedMaterial;
        }

        public void DeSelect()
        {
            //ObjectRenderer.material = UnClickedMaterial;
        }

        public void Move(Vector3 target)
        {
            currTarget = target;
            velocity = Vector3.Normalize(target - transform.position);
        }
    }

}
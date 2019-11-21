using UnityEngine;

namespace FIVE
{
    public class Bullet : MonoBehaviour
    {
        public Vector3 Target
        {
            get => _target;
            set
            {
                _target = value;
                transform.up = Vector3.Normalize(_target - transform.position);
            }
        }

        private Vector3 _target;

        private float elapsedTime;

        private void Start()
        {
            elapsedTime = 0;
        }

        private void Update()
        {
            //Debug.Log(transform.position);
            elapsedTime += Time.deltaTime;
            if (elapsedTime > 5.0f)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
            else
            {
                if (Target != null)
                {
                    transform.Translate(Vector3.up * 3.0f);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.name.StartsWith("robotSphere"))
            {
                if (other.gameObject.name.StartsWith("AlienBeetle"))
                {
                    EnemyBehavior enemyBehavior = other.GetComponent<EnemyBehavior>();
                    enemyBehavior.OnHit();
                }
                GetComponentInChildren<TrailRenderer>().enabled = false;
                GetComponentInChildren<ParticleSystem>().Play();
                Destroy(gameObject, 1f);
            }
        }
    }
}
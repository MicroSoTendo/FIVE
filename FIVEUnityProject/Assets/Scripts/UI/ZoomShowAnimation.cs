using System;
using UnityEngine;

namespace FIVE.UI
{
    public class ZoomShowAnimation : MonoBehaviour
    {
        public EventHandler OnFinished = (o, e) => { };
        private float speed;
        private readonly float acceleration = 1f;
        private Action OnUpdate = () => { };
        private bool started = false;

        private void Start()
        {
            transform.localScale = Vector3.zero;
        }

        public void SetUpSpeed(float newSpeed = 1f)
        {
            speed = newSpeed;
        }

        public void StartZoomIn()
        {
            if (started)
            {
                return;
            }

            OnUpdate = DoZoomIn;
            started = true;
        }

        private void UpdateSpeed()
        {
            speed += acceleration * Time.deltaTime;
        }

        private void DoZoomIn()
        {
            UpdateSpeed();
            float step = speed * Time.deltaTime;
            transform.localScale += new Vector3(1, 1, 1) * step;
            if (transform.localScale.x >= 1)
            {
                OnFinished?.Invoke(this, EventArgs.Empty);
                OnUpdate = DoDynamicScale;
            }
        }


        private float timer = 0;
        private void DoDynamicScale()
        {
            transform.localScale = new Vector3(1, 1, 1) * (0.7f + 0.275f * ((Mathf.Cos(timer) + 1f) / 1.85f));
            timer += Time.deltaTime;
        }

        private void Update()
        {
            OnUpdate.Invoke();
        }
    }
}

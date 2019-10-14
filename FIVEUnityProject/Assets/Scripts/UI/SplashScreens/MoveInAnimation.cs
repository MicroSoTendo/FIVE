using System;
using UnityEngine;

namespace FIVE.UI.SplashScreens
{
    public class MoveInAnimation : MonoBehaviour
    {
        private float acceleration;
        public EventHandler OnFinished = (o, e) => { };
        private Action OnUpdate = () => { };
        private float speed;
        private bool started = false;

        private Vector3 targetPosition;

        //private static bool firstStarted = false;
        public void SetTargetAndSpeed(Vector3 newTargetLocation, float newSpeed = 1f, float newAcceleration = 900f)
        {
            targetPosition = newTargetLocation;
            speed = newSpeed;
            acceleration = newAcceleration;
        }

        public void StartMoving()
        {
            if (started)
            {
                return;
            }

            OnUpdate = DoMoving;
            started = true;
        }

        private void UpdateSpeed()
        {
            speed += Time.deltaTime * acceleration;
        }

        private void DoMoving()
        {
            UpdateSpeed();
            float step = speed * Time.deltaTime;
            transform.localPosition =
                Vector3.MoveTowards(transform.localPosition, targetPosition, step);
            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.001f)
            {
                OnFinished?.Invoke(this, EventArgs.Empty);
                Destroy(this);
            }
        }

        private void Update()
        {
            OnUpdate.Invoke();
        }
    }
}
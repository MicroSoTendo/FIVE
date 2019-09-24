using System;
using System.Collections;
using FIVE.EventSystem;
using UnityEngine;

namespace FIVE.UI.SplashScreens
{
    public abstract class OnProgressUpdated : IEventType<OnProgressUpdatedEventArgs> { }

    public class OnProgressUpdatedEventArgs : EventArgs
    {
        public float Progress { get; }
        public OnProgressUpdatedEventArgs(float progress) => Progress = progress;
    }

    public abstract class LoadingSplashScreen : SplashScreen
    {
        protected float progress;
        public float Progress { get => progress; set => progress = Mathf.Clamp(value, 0f, 1f); }

        protected LoadingSplashScreen()
        {
            progress = 0;
            EventManager.Subscribe<OnProgressUpdated, OnProgressUpdatedEventArgs>(OnProgressUpdated);
        }

        private void OnProgressUpdated(object sender, OnProgressUpdatedEventArgs e)
        {
            progress = e.Progress;
        }

        protected abstract void UpdateLoadingProgressBar();
        public override IEnumerator OnTransitioning()
        {
            while (Progress <= 1)
            {
                UpdateLoadingProgressBar();
                yield return null;
            }
        }
    }
}

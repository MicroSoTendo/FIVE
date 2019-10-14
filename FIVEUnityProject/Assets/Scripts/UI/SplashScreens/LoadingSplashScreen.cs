using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FIVE.EventSystem;

namespace FIVE.UI.SplashScreens
{
    public class ProgressEventArgs : EventArgs
    {
        public float Progress { get; }
        public ProgressEventArgs(float progress) => Progress = progress;
    }

    public abstract class OnProgress : IEventType<ProgressEventArgs>
    {
    }

    public abstract class LoadingSplashScreen : ISplashScreen
    {
        private readonly Dictionary<object, float> progressValues;
        protected float Progress { get; private set; }

        protected LoadingSplashScreen()
        {
            Progress = 0;
            progressValues = new Dictionary<object, float>();
            EventManager.Subscribe<OnProgress, ProgressEventArgs>(OnProgressChanged);
        }

        public IEnumerator TransitionRoutine()
        {
            while (Progress <= 1)
            {
                UpdateLoadingProgressBar();
                yield return null;
            }

            progressValues.Clear();
        }

        protected abstract void UpdateLoadingProgressBar();

        private void OnProgressChanged(object sender, ProgressEventArgs e)
        {
            if (progressValues.ContainsKey(sender))
            {
                progressValues[sender] = e.Progress;
            }
            else
            {
                progressValues.Add(sender, e.Progress);
            }

            Progress = progressValues.Values.Sum() / progressValues.Count;
        }
    }
}
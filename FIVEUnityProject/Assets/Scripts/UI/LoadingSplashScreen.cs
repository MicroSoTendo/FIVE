using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.UI
{
    public abstract class LoadingSplashScreen : SplashScreen
    {
        protected Queue<Action> loadingTasks;
        protected int totalTasks;
        protected int finishedTasks;

        protected LoadingSplashScreen(Queue<Action> loadingLoadingTasks)
        {
            loadingTasks = loadingLoadingTasks;
            totalTasks = loadingLoadingTasks.Count;
            finishedTasks = 0;
        }
        protected abstract void UpdateLoadingProgressBar();
        public override IEnumerator OnTransitioning()
        {
            while (loadingTasks.Count > 0)
            {
                Action action = loadingTasks.Dequeue();
                action();
                finishedTasks++;
                UpdateLoadingProgressBar();
                yield return null;
            }
        }
    }
}

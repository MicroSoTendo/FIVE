using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class LoadingSplashScreen : SplashScreen
    {


        public class LoadingGUI : MonoBehaviour
        {

            public Action OnGuiAction;
            private float progress;
            /// <summary>
            /// </summary>
            /// <param name="newProgress">range from 0 to 1</param>
            public void SetProgress(float newProgress)
            {
                progress = newProgress;
            }

            void OnGUI()
            {
                OnGuiAction();
            }
        }
        public class Wrap { }
        public class Wrap<T> : Wrap
        {
            public T _()
            {
                return default;
            }
        }

        private readonly Queue<Action> loadingTasks;
        private readonly int totalTasks;

        private bool isFinished = false;

        Wrap<Action<Wrap>> _ = new Wrap<Action<Wrap>>();
        private LoadingGUI loadingGui;
        public LoadingSplashScreen(Queue<Action> loadingLoadingTasks, LoadingGUI loadingGuiBehaviour)
        {
            this.loadingTasks = loadingLoadingTasks;
            totalTasks = loadingLoadingTasks.Count;
            loadingGui = loadingGuiBehaviour;
        }

        private bool IsFinished() => isFinished;
        public override IEnumerator OnTransitioning()
        {
            for (int i = 0; i < totalTasks; i++)
            {
                loadingTasks.Dequeue()();
                loadingGui.SetProgress((float)loadingTasks.Count / totalTasks);
                yield return new WaitUntil(IsFinished);
            }

            isFinished = true;
            yield return new WaitUntil(IsFinished);
        }


    }
}

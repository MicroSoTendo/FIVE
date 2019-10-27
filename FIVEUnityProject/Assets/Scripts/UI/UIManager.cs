using FIVE.UI.SplashScreens;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FIVE.EventSystem.Util;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        public enum CursorType
        {
            Aim,
            Regular,
            Hidden
        }

        private static readonly Dictionary<string, ViewModel> NameToVMs = new Dictionary<string, ViewModel>();

        private static readonly Dictionary<Type, SortedSet<ViewModel>> TypeToVMs =
            new Dictionary<Type, SortedSet<ViewModel>>();

        private static Texture2D cursorTexture;

        public static void SetCursor(CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.Aim:
                    Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    Cursor.visible = true;
                    break;
                case CursorType.Regular:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    Cursor.visible = true;
                    break;
                case CursorType.Hidden:
                    Cursor.visible = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cursorType), cursorType, null);
            }
        }

        public static ViewModel GetOrCreate<T>(string name = null) where T : ViewModel
        {
            ViewModel vm = Get<T>(name) ?? Create<T>(name);
            return vm;
        }

        public static T Get<T>(string name = null) where T : ViewModel
        {
            if (name != null)
            {
                return (T)NameToVMs[name];
            }

            if (TypeToVMs.TryGetValue(typeof(T), out SortedSet<ViewModel> values))
            {
                return (T)values.First();
            }

            return null;
        }

        private void Awake()
        {
            Coroutine vmCoroutine = null;
            vmCoroutine = StartCoroutine(ViewModel.InitializeRoutine(ProgressCallBack));

            void ProgressCallBack(float progress)
            {
                vmCoroutine.RaiseEvent<OnProgress, ProgressEventArgs>(new ProgressEventArgs(progress));
            }

            var startUpScreen = new StartUpScreen();
            StartCoroutine(startUpScreen.TransitionRoutine());
            cursorTexture = Resources.Load<Texture2D>("Textures/UI/Cursor");
        }


        public static T Create<T>(string name = null) where T : ViewModel
        {
            if (!TypeToVMs.ContainsKey(typeof(T)))
            {
                TypeToVMs.Add(typeof(T), new SortedSet<ViewModel>(new ViewModel.ViewModelComparer()));
            }

            var newViewModel = ViewModel.Create<T>();
            NameToVMs.Add(name ?? typeof(T).Name, newViewModel);
            TypeToVMs[typeof(T)].Add(newViewModel);
            return (T)newViewModel;
        }
    }
}
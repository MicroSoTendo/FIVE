using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public abstract partial class ViewModel
    {
        private static readonly Canvas[] Canvas;
        private static readonly GameObject ViewModelsRoot;
        private static readonly ConcurrentDictionary<Type, ViewModel> CachedViewModels;
        public static bool Initialized { get; private set; }

        static ViewModel()
        {
            Initialized = false;
            ViewModelsRoot = new GameObject(nameof(ViewModelsRoot));
            CanvasGroup canvasGroup = ViewModelsRoot.AddComponent<CanvasGroup>();
            Canvas = new Canvas[3];
            InitCanvas();
            CachedViewModels = new ConcurrentDictionary<Type, ViewModel>();
        }

        private static void InitCanvas()
        {
            foreach (object value in Enum.GetValues(typeof(RenderMode)))
            {
                var go = new GameObject { name = value.ToString() };
                go.transform.SetParent(ViewModelsRoot.transform);
                Canvas c = go.AddComponent<Canvas>();
                go.AddComponent<CanvasScaler>();
                GraphicRaycaster graphicRaycaster = go.AddComponent<GraphicRaycaster>();

                Canvas[(int)value] = c;
                c.renderMode = (RenderMode)value;
            }
        }

        public static IEnumerator InitializeRoutine(Action<float> progressCallback)
        {


            Type[] types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                            from type in assembly.GetTypes()
                            where !type.IsAbstract && typeof(ViewModel).IsAssignableFrom(type)
                            select type).ToArray();
            int total = types.Length;
            int finished = 0;
            foreach (Type type in types)
            {
                var cached = (ViewModel)type.GetConstructor(Type.EmptyTypes)?.Invoke(null);
                CachedViewModels.TryAdd(type, cached);
                finished++;
                progressCallback((float)finished / total);
                yield return null;
            }

            Initialized = true;
        }

        public static ViewModel Create<T>() where T : ViewModel
        {
            CachedViewModels.TryGetValue(typeof(T), out ViewModel vm);
            return vm;
        }
    }
}
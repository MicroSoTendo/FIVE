using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public abstract class View
    {
        public Canvas ViewCanvas { get; set; }

        public Vector2 Size
        {
            get
            {
                return ViewCanvas.pixelRect.size;
            }
            set
            {

                //ViewCanvas.gameObject.GetComponent<CanvasScaler>().
            }

        }
        protected GameObject parent;
        protected CanvasScaler canvasScaler;
        protected GraphicRaycaster graphicRaycaster;
        protected Dictionary<string, GameObject> nameToUIElementGameObjects;
        protected XMLDeserializer xmlDeserializer;
        protected View()
        {
            parent = new GameObject { name = GetType().Name };
            ViewCanvas = parent.AddComponent<Canvas>();
            canvasScaler = parent.AddComponent<CanvasScaler>();
            graphicRaycaster = parent.AddComponent<GraphicRaycaster>();
            nameToUIElementGameObjects = new Dictionary<string, GameObject>();
            xmlDeserializer = new XMLDeserializer($"UI/{GetType().Name}", ViewCanvas);
            ViewCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        protected T AddUIElement<T>(string name) where T : MonoBehaviour
        {
            xmlDeserializer.Deserialize(name, out GameObject gameObject, out T UIElement);
            nameToUIElementGameObjects.Add(name, gameObject);
            return UIElement;
        }

    }
    public abstract class View<TView, TViewModel> : View
        where TView : View<TView, TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {
        private static readonly Dictionary<Type, View> CachedViews = new Dictionary<Type, View>();
        public static T Create<T>() where T : View<TView, TViewModel>, new()
        {
            var newView = new T();
            if (CachedViews.ContainsKey(typeof(T)))
            {
                return CachedViews[typeof(T)] as T;
            }
            return newView;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public abstract class View
    {
        public Canvas ViewCanvas { get; set; }
        public Dictionary<string, GameObject> Resources { get; }
        protected GameObject canvasGameObject;
        protected CanvasScaler canvasScaler;
        protected GraphicRaycaster graphicRaycaster;
        protected Dictionary<string, GameObject> nameToUIElementGameObjects;
        protected XMLDeserializer xmlDeserializer;
        protected View()
        {
            canvasGameObject = new GameObject { name = GetType().Name };
            ViewCanvas = canvasGameObject.AddComponent<Canvas>();
            canvasScaler = canvasGameObject.AddComponent<CanvasScaler>();
            graphicRaycaster = canvasGameObject.AddComponent<GraphicRaycaster>();
            nameToUIElementGameObjects = new Dictionary<string, GameObject>();
            Resources = new Dictionary<string, GameObject>();
            xmlDeserializer = new XMLDeserializer($"UI/{GetType().Name}", ViewCanvas);
            ViewCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        public T AddUIElement<T>(string name)
        {
            xmlDeserializer.Deserialize(name, out GameObject gameObject);
            nameToUIElementGameObjects.Add(name, gameObject);
            return gameObject is T go ? go : gameObject.GetComponent<T>();
        }

        public T AddUIElementFromResources<T>(string resourceName, string gameObjectName, Transform parent)
        {
            GameObject prefab = Resources[resourceName];
            GameObject gameObject = GameObject.Instantiate(prefab, parent);
            gameObject.name = gameObjectName;
            nameToUIElementGameObjects.Add(gameObjectName, gameObject);
            return gameObject is T go ? go : gameObject.GetComponent<T>();
        }

        public void RemoveUIElement(string name)
        {
            var go = nameToUIElementGameObjects[name];
            nameToUIElementGameObjects.Remove(name);
            go.transform.SetParent(null);
            go.SetActive(false);
            
            Debug.Log($"{nameof(RemoveUIElement)} {go.name}");
            GameObject.Destroy(go);
        }

        public T GetUIElement<T>(string name, bool includeInactive = false) where T : MonoBehaviour
        {
            return canvasGameObject.GetComponentsInChildren(typeof(T), includeInactive).Cast<T>().FirstOrDefault(child => child.name == name);
        }

        protected void LoadResources()
        {
            xmlDeserializer.LoadResources(Resources);
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
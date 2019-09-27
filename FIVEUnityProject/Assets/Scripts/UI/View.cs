using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public abstract class View
    {
        public Canvas ViewCanvas { get; set; }
        public Dictionary<string, GameObject> CanvasResources { get; }
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
            CanvasResources = new Dictionary<string, GameObject>();
#if DEBUG
            string xmlText = File.ReadAllText($"{Application.dataPath}/Resources/UI/{GetType().Name}.xml");
#else
            var xmlText = Resources.Load<TextAsset>($"UI/{GetType().Name}").text;
#endif
            xmlDeserializer = new XMLDeserializer(xmlText, ViewCanvas);
            ViewCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            AutoLoad();
            LoadResources();
        }

        private void AutoLoad()
        {
            IEnumerable<PropertyInfo> uiElementPropertyInfos = GetType().GetProperties().Where(prop => prop.IsDefined(typeof(UIElementAttribute), false));
            foreach (PropertyInfo uiElementPropertyInfo in uiElementPropertyInfos)
            {
                Type type = uiElementPropertyInfo.PropertyType;
                string propertyName = uiElementPropertyInfo.Name;
                string xmlNodeName = uiElementPropertyInfo.GetCustomAttribute<UIElementAttribute>().Path;
                uiElementPropertyInfo.SetValue(this, AddUIElement(xmlNodeName ?? propertyName, type));
            }
        }

        public object AddUIElement(string name, Type type)
        {
            xmlDeserializer.Deserialize(name, out GameObject gameObject);
            nameToUIElementGameObjects.Add(name, gameObject);
            return gameObject.GetType() == type ? gameObject : (object)gameObject.GetComponent(type);
        }

        public T AddUIElement<T>(string name)
        {
            xmlDeserializer.Deserialize(name, out GameObject gameObject);
            nameToUIElementGameObjects.Add(name, gameObject);
            return gameObject is T go ? go : gameObject.GetComponent<T>();
        }

        public T AddUIElementFromResources<T>(string resourceName, string gameObjectName, Transform parent)
        {
            GameObject prefab = CanvasResources[resourceName];
            GameObject gameObject = GameObject.Instantiate(prefab, parent);
            gameObject.SetActive(true);
            gameObject.name = gameObjectName;
            nameToUIElementGameObjects.Add(gameObjectName, gameObject);
            return gameObject is T go ? go : gameObject.GetComponent<T>();
        }

        public void RemoveUIElement(string name)
        {
            GameObject go = nameToUIElementGameObjects[name];
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
#if DEBUG
        public void Unload()
        {
            foreach (GameObject gameObject in nameToUIElementGameObjects.Values.ToArray())
            {
                gameObject.SetActive(false);
                GameObject.Destroy(gameObject);
            }

            foreach (GameObject gameObject in CanvasResources.Values)
            {

                gameObject.SetActive(false);
                GameObject.Destroy(gameObject);
            }
            nameToUIElementGameObjects.Clear();
            CanvasResources.Clear();
            GameObject.Destroy(canvasGameObject);
        }
#endif
        protected void LoadResources()
        {
            xmlDeserializer.LoadResources(CanvasResources);
        }
    }

    public abstract class View<TView, TViewModel> : View
        where TView : View<TView, TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {
        private static readonly Dictionary<Type, View> CachedViews = new Dictionary<Type, View>();
        public static T Create<T>() where T : View<TView, TViewModel>, new()
        {
            T newView = new T();
            if (CachedViews.ContainsKey(typeof(T)))
            {
                return CachedViews[typeof(T)] as T;
            }
            return newView;
        }
    }
}
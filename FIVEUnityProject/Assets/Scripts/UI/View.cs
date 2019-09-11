using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FIVE.UI
{
    public abstract class View
    {
        public RenderMode RenderMode { get; set; }
        protected GameObject parent;
        protected Canvas canvas;
        protected CanvasScaler canvasScaler;
        protected GraphicRaycaster graphicRaycaster;
        protected Dictionary<string, GameObject> nameToUIElementGO;

        protected XmlDocument viewXml;
        protected View()
        {
            parent = new GameObject();
            parent.name = GetType().Name;
            canvas = parent.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasScaler = parent.AddComponent<CanvasScaler>();
            graphicRaycaster = parent.AddComponent<GraphicRaycaster>();
            nameToUIElementGO = new Dictionary<string, GameObject>();

            var pathToXml = $"UI/{GetType().Name}";// + ".xml";
            var xmlFile = Resources.Load<TextAsset>(pathToXml);
            viewXml = new XmlDocument();
            viewXml.LoadXml(xmlFile.text);
        }

        protected T AddUIElement<T>(string name = "") where T : MonoBehaviour
        {
            if (name.Length != 0)
            {
                var xmlNode = viewXml.SelectNodes($"/Canvas/Button[@name='{name}']")?[0];
                var prefabAttribute = xmlNode?.Attributes?["prefab"];
                var prefabPath = prefabAttribute?.InnerText;
                var prefab = Resources.Load<GameObject>(prefabPath);
                var go = Object.Instantiate(prefab, canvas.transform);
                nameToUIElementGO.Add(name, go);
                return go.GetComponent<T>();
            }
            var newUIElementGO = new GameObject();
            newUIElementGO.transform.parent = canvas.transform;
            var newUIElement = newUIElementGO.AddComponent<T>();
            if (name.Length == 0)
            {
                name = typeof(T) + newUIElementGO.GetInstanceID().ToString();
            }
            newUIElementGO.name = name;
            nameToUIElementGO.Add(name, newUIElementGO);
            return newUIElement;
        }

    }
    public abstract class View<TView, TViewModel> : View
        where TView : View<TView, TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {
        private static Dictionary<Type, View> cachedViews = new Dictionary<Type, View>();
        public static T Create<T>() where T : View<TView, TViewModel>, new()
        {
            return new T();
            T newView = new T();
            if (cachedViews.ContainsKey(typeof(T)))
            {
                return cachedViews[typeof(T)] as T;
            }
            return newView;
        }

        private static void SetAttributesFromXml<T>(XmlDocument xmlDocument, T uiElement) where T : MonoBehaviour
        {
            
        }

    }
}
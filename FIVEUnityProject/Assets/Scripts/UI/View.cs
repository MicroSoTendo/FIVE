using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public abstract class View<TView, TViewModel>
        where TView : View<TView, TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {
        private static Dictionary<string, View<TView, TViewModel>> cachedViews = new Dictionary<string, View<TView, TViewModel>>();

        public static T Create<T>() where T : View<TView, TViewModel>, new()
        {
            T newView = new T();
            //TODO: Xml or 3rd layout packages
            var fileName = typeof(T).Name + "Layout.xml";
            if (cachedViews.ContainsKey(fileName))
            {
                //TODO: Load from cache
            }
            var xmlFile = Resources.Load<TextAsset>($"UI/{fileName}");
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlFile.text);
            foreach (var keyValuePair in newView.nameToUIElement)
            {
                var name = keyValuePair.Key;
                var uiElement = keyValuePair.Value;
                //TODO: Deserialize config from xml to UIElements
            }
            return newView;
        }

        public RenderMode RenderMode { get; set; }
        protected GameObject parent;
        protected Canvas canvas;
        protected CanvasScaler canvasScaler;
        protected GraphicRaycaster graphicRaycaster;
        protected Dictionary<string, MonoBehaviour> nameToUIElement;

        protected View()
        {
            parent = new GameObject();
            canvas = parent.AddComponent<Canvas>();
            canvasScaler = parent.AddComponent<CanvasScaler>();
            graphicRaycaster = parent.AddComponent<GraphicRaycaster>();
        }

        protected T AddUIElement<T>(string name = "") where T : MonoBehaviour
        {
            var newUIElement = parent.AddComponent<T>();
            if (name.Length == 0)
            {
                name = typeof(T) + newUIElement.GetInstanceID().ToString();
            }
            nameToUIElement.Add(name, newUIElement);
            return newUIElement;
        }

        private static void SetAttributesFromXml<T>(XmlDocument xmlDocument, T uiElement) where T : MonoBehaviour
        {
            
        }

    }
}
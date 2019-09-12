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
        protected Dictionary<string, GameObject> nameToUIElementGameObjects;

        protected XmlDocument viewXml;
        protected View()
        {
            parent = new GameObject { name = GetType().Name };
            canvas = parent.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasScaler = parent.AddComponent<CanvasScaler>();
            graphicRaycaster = parent.AddComponent<GraphicRaycaster>();
            nameToUIElementGameObjects = new Dictionary<string, GameObject>();

            string pathToXml = $"UI/{GetType().Name}";
            var xmlFile = Resources.Load<TextAsset>(pathToXml);
            viewXml = new XmlDocument();
            viewXml.LoadXml(xmlFile.text);
        }

        private readonly Dictionary<string, Func<XmlAttribute, object>> attributeParser = new Dictionary<string, Func<XmlAttribute, object>>
        {
            {"text", attribute => attribute.InnerText},
            {"position", attribute => { string[] positionAttribute = attribute.InnerText.Split(','); 
                                        float x = float.Parse(positionAttribute[0]), 
                                              y = float.Parse(positionAttribute[1]), 
                                              z = float.Parse(positionAttribute[2]);
                                        return new Vector3(x,y,z); }},
            {"prefab", attribute => Resources.Load<GameObject>(attribute.InnerText)}
        };

        private readonly Dictionary<string, Action<GameObject, object>> attributeHandler = new Dictionary<string, Action<GameObject, object>>
        {
            {"text", (gameObject, attribute)=>{ (gameObject.GetComponent<Text>() ?? gameObject.GetComponentInChildren<Text>()).text = (string)attribute;}},
            {"position", (gameObject, attribute)=>{ gameObject.transform.localPosition = (Vector3)attribute;}},
        };

        private Dictionary<string, object> ParseAttributes(XmlAttributeCollection attributes)
        {
            var parsedAttributes = new Dictionary<string, object>();
            foreach (XmlAttribute attribute in attributes)
            {
                string name = attribute.Name;
                if (attributeParser.ContainsKey(name))
                {
                    parsedAttributes.Add(name, attributeParser[name](attribute));
                }
            }
            return parsedAttributes;
        }
        private void DeserializeFromXml<T>(string name, out GameObject gameObject, out T UIElement) where T : MonoBehaviour
        {
            //Try find definition in Xml
            XmlNode xmlNode = viewXml.SelectSingleNode($"/Canvas/{typeof(T).Name}[@name='{name}']");
            if (xmlNode == null)
            {
                gameObject = default;
                UIElement = default;
                return;
            }
            //Load attributes
            Dictionary<string, object> parsedAttributes = ParseAttributes(xmlNode.Attributes);
            //Initialize gameobject
            if (parsedAttributes.ContainsKey("prefab"))
            {
                gameObject = Object.Instantiate(parsedAttributes["prefab"] as GameObject);
                parsedAttributes.Remove("prefab");
            }
            else
            {
                gameObject = Object.Instantiate(Resources.Load<GameObject>($"EntityPrefabs/UIPrimitives/{typeof(T).Name}"));
            }
            UIElement = gameObject.GetComponent<T>();
            gameObject.name = name;
            gameObject.transform.SetParent(canvas.transform);

            foreach (KeyValuePair<string, object> keyValue in parsedAttributes)
            {
                attributeHandler[keyValue.Key](gameObject, keyValue.Value);
            }
        }

        protected T AddUIElement<T>(string name) where T : MonoBehaviour
        {
            DeserializeFromXml(name, out GameObject gameObject, out T UIElement);
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

        private static void SetAttributesFromXml<T>(XmlDocument xmlDocument, T uiElement) where T : MonoBehaviour
        {

        }

    }
}
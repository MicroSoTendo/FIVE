using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FIVE.UI
{
    public partial class XMLDeserializer
    {
        private readonly XmlDocument viewXml;
        private readonly Canvas parentCanvas;
        private readonly string xmlText;
        private readonly Dictionary<XmlNode, GameObject> deserializedGameObjects = new Dictionary<XmlNode, GameObject>();
        public XMLDeserializer(XmlDocument viewXml, Canvas parentCanvas)
        {
            this.viewXml = viewXml;
            this.parentCanvas = parentCanvas;
        }
        public XMLDeserializer(string xmlText, Canvas parentCanvas)
        {
            viewXml = new XmlDocument();
            viewXml.LoadXml(xmlText);
            this.parentCanvas = parentCanvas;
        }

        private Type PrimitiveHelper(string typeName)
        {
            switch (typeName)
            {
                case nameof(Button): return typeof(Button);
                case nameof(Dropdown): return typeof(Dropdown);
                case nameof(Image): return typeof(Image);
                case nameof(InputField): return typeof(InputField);
                case nameof(Scrollbar): return typeof(Scrollbar);
                case nameof(Slider): return typeof(Slider);
                case nameof(Text): return typeof(Text);
                case nameof(Toggle): return typeof(Toggle);
                case nameof(GameObject): return typeof(GameObject);
                default: return Type.GetType(typeName);
            }
        }

        public void LoadResources(Dictionary<string, GameObject> resources)
        {
            XmlNode xmlNode = viewXml.SelectSingleNode("/Canvas/Resources");
            if (xmlNode == null)
            {
                return;
            }

            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                GameObject gameObject = DeserializeHelper(childNode, isResource: true);
                resources.Add(gameObject.name, gameObject);
            }
        }

        public void Deserialize(string name, out GameObject deserializedUIElement)
        {
            //Try find definition in Xml
            XmlNode xmlNode = viewXml.SelectSingleNode($"/Canvas//*[@name='{name}']");
            if (xmlNode == null)
            {
                deserializedUIElement = default;
                return;
            }
            GameObject go = DeserializeHelper(xmlNode);
            deserializedGameObjects.Add(xmlNode, go);
            deserializedUIElement = go;
        }

        private GameObject DeserializeHelper(XmlNode xmlNode, bool isResource = false)
        {
            GameObject uiElement = null;
            Dictionary<string, object> parsedAttributes = ParseAttributes(xmlNode.Attributes);
            string typeName = xmlNode.Name;
            bool isPrefab = parsedAttributes.ContainsKey("Prefab");
            //Check if load from prefab
            if (isPrefab)
            {
                uiElement = isResource ? parsedAttributes["Prefab"] as GameObject : Object.Instantiate(parsedAttributes["Prefab"] as GameObject, parentCanvas.transform);
                parsedAttributes.Remove("Prefab");
            }
            else
            {
                string primitivePath = $"EntityPrefabs/UIPrimitives/{typeName}";
                uiElement = Object.Instantiate(Resources.Load<GameObject>(primitivePath), parentCanvas.transform);
            }

            if (xmlNode.ParentNode != null && deserializedGameObjects.ContainsKey(xmlNode.ParentNode))
            {
                uiElement.transform.SetParent(deserializedGameObjects[xmlNode.ParentNode].transform);
            }

            //Apply Attributes to UIElement
            foreach (KeyValuePair<string, object> keyValue in parsedAttributes)
            {
                if (AttributeHandler.ContainsKey(keyValue.Key))
                {
                    AttributeHandler[keyValue.Key](uiElement, keyValue.Value);
                }
                else
                {
                    GeneralPropertyHandler(uiElement, keyValue.Key, keyValue.Value as List<(string, object)>);
                }
            }

            //foreach (XmlNode node in xmlNode.ChildNodes)
            //{
            //    DeserializeHelper(node, false);
            //}

            return uiElement;
        }
    }
}
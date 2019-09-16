using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FIVE.UI
{
    public partial class XMLDeserializer
    {
        private readonly XmlDocument viewXml;
        private readonly Canvas parentCanvas;
        public XMLDeserializer(XmlDocument viewXml, Canvas parentCanvas)
        {
            this.viewXml = viewXml;
            this.parentCanvas = parentCanvas;
        }
        public XMLDeserializer(string pathToXml, Canvas parentCanvas)
        {
            TextAsset xmlFile = Resources.Load<TextAsset>(pathToXml);
            viewXml = new XmlDocument();
            viewXml.LoadXml(xmlFile.text);
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

        //private void 

        public void LoadResources(Dictionary<string, GameObject> resources)
        {
            XmlNode xmlNode = viewXml.SelectSingleNode("/Canvas/Resources");
            if (xmlNode == null) return;
            foreach (XmlNode childNode in xmlNode.ChildNodes)
            {
                GameObject gameObject = DeserializeHelper(childNode);
                gameObject.SetActive(false);
                resources.Add(gameObject.name, gameObject);
            }
        }

        public void Deserialize(string name, out GameObject gameObject)
        {
            //Try find definition in Xml
            XmlNode xmlNode = viewXml.SelectSingleNode($"/Canvas/*[@name='{name}']");
            if (xmlNode == null)
            {
                gameObject = default;
                return;
            }
            gameObject = DeserializeHelper(xmlNode);
        }


        private GameObject DeserializeHelper(XmlNode xmlNode, GameObject root = null)
        {
            GameObject gameObject = root;
            //Load attributes
            Dictionary<string, object> parsedAttributes = ParseAttributes(xmlNode.Attributes);
            string typeName = xmlNode.Name;
            //Check if load from prefab
            if (gameObject == null)
            {
                if (parsedAttributes.ContainsKey("Prefab"))
                {
                    gameObject = Object.Instantiate(parsedAttributes["Prefab"] as GameObject, parentCanvas.transform);
                    parsedAttributes.Remove("Prefab");
                }
                else
                {
                    string primitivePath = $"EntityPrefabs/UIPrimitives/{typeName}";
                    gameObject = Object.Instantiate(Resources.Load<GameObject>(primitivePath), parentCanvas.transform);
                }
            }
        
            //Set attributes to UIElement
            foreach (KeyValuePair<string, object> keyValue in parsedAttributes)
            {
                if (AttributeHandler.ContainsKey(keyValue.Key))
                    AttributeHandler[keyValue.Key].DynamicInvoke(gameObject, keyValue.Value);
                else
                    GeneralPropertyHandler(gameObject, keyValue.Key, keyValue.Value as List<(string, object)>);
            }
            //Set ChildNode
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                var childRoot = gameObject.GetComponentsInChildren(PrimitiveHelper(node.Name))
                    .FirstOrDefault(o => o.name == node.Attributes["name"].InnerText).gameObject;
                DeserializeHelper(node, childRoot);
            }
            return gameObject;
        }

    }
}
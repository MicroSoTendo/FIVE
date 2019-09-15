using System.Collections.Generic;
using System.Xml;
using UnityEngine;

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
        public void Deserialize<T>(string name, out GameObject gameObject, out T UIElement) where T : MonoBehaviour
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
            //Initialize GameObject/Check if load from prefab
            if (parsedAttributes.ContainsKey(nameof(GameObject)))
            {
                gameObject = GameObject.Instantiate(parsedAttributes[nameof(GameObject)] as GameObject);
                parsedAttributes.Remove(nameof(GameObject));
            }
            else
            {
                gameObject = GameObject.Instantiate(Resources.Load<GameObject>($"EntityPrefabs/UIPrimitives/{typeof(T).Name}"));
            }
            UIElement = gameObject.GetComponent<T>();
            gameObject.name = name;
            gameObject.transform.SetParent(parentCanvas.transform);

            foreach (KeyValuePair<string, object> keyValue in parsedAttributes)
            {
                if (AttributeHandler.ContainsKey(keyValue.Key))
                {
                    AttributeHandler[keyValue.Key].DynamicInvoke(gameObject, keyValue.Value);
                }
                else
                {
                    GeneralPropertyHandler(gameObject, keyValue.Key, keyValue.Value as List<(string, object)>);
                }
            }
        }
    }
}
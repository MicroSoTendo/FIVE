using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public partial class XMLDeserializer
    {
        private static readonly Dictionary<string, Func<XmlAttribute, object>> AttributeParser;
        private static readonly Dictionary<string, Delegate> AttributeHandler;
        private static readonly Dictionary<string, Delegate> PropertyParserDictionary;

        static XMLDeserializer()
        {

            AttributeParser = new Dictionary<string, Func<XmlAttribute, object>>
            {
                {"name", x => x.InnerText },
                {"Prefab", PrefabParser},
                {"NativeSize", (a)=>
                    {
                        bool.TryParse(a.InnerText, out bool result);
                        return result;
                    }
                },
                {nameof(Text), TextParser},
                {nameof(Transform), PropertyParser},
                {nameof(Sprite), SpriteParser},
                {nameof(RectTransform), PropertyParser},
            };

            AttributeHandler = new Dictionary<string, Delegate>
            {
                {"name", (Action<GameObject,string>)((g, s) => { g.name = s;})},
                {"NativeSize", (Action<GameObject,bool>)((g, b) => {
                    if (b)
                    {
                        Debug.Log("SetToNative");
                        g.GetComponent<Image>().SetNativeSize();
                    }})},
                {nameof(Text), (Action<GameObject, List<(string, object)>>)TextHandler},
                {nameof(Transform), (Action<GameObject, List<(string, object)>>)TransformHandler},
                {nameof(Sprite), (Action<GameObject,Sprite>)SpriteHandler},
                {nameof(RectTransform), (Action<GameObject,List<(string,object)>>)RectTransformHandler},
            };

            PropertyParserDictionary = new Dictionary<string, Delegate>()
            {
                {"anchorMin", (Func<string, Vector2>)Vector2Parser},
                {"anchorMax", (Func<string, Vector2>)Vector2Parser},
                {"pivot", (Func<string, Vector2>)Vector2Parser},
                {"sizeDelta", (Func<string, Vector2>)Vector2Parser},
                {"anchoredPosition", (Func<string, Vector2>)Vector2Parser},
                {"anchoredPosition3D", (Func<string, Vector3>)Vector3Parser},
                {"localPosition", (Func<string, Vector3>)Vector3Parser},
                {"position", (Func<string, Vector3>)Vector3Parser},
                {"offsetMax", (Func<string, Vector2>)Vector2Parser},
                {"offsetMin", (Func<string, Vector2>)Vector2Parser},
            };
        }

        private static List<(string, object)> PropertyParser(XmlAttribute attribute)
        {
            List<(string, object)> parsedObjects = new List<(string, object)>();
            Regex r = new Regex(@"\s*([\w]+)\s*\:\s*([\w\,\-\+\(\)]+)\;\s*");
            MatchCollection matchCollection = r.Matches(attribute.InnerText);
            foreach (Match match in matchCollection)
            {
                GroupCollection captures = match.Groups;
                string name = captures[1].Value;
                string value = captures[2].Value;
                object parsedValue = PropertyParserDictionary[name].DynamicInvoke(value);
                parsedObjects.Add((name, parsedValue));
            }
            return parsedObjects;
        }

        private static void GeneralPropertyHandler(GameObject gameObject, string type, List<(string, object)> parsedObjects)
        {
            foreach ((string propertyName, object value) in parsedObjects)
            {
                Component component = gameObject.GetComponent(type);
                component?.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty)?.SetValue(component, value);
            }
        }

        private static bool TryBinding(XmlAttribute xmlAttribute)
        {
            //TODO: Finish
            string text = xmlAttribute.InnerText;
            Regex directBindRegex = new Regex(@"\{Binding\s+([\w_]+)\s*\}");
            Regex pathBindRegex = new Regex(@"\{Binding\s+([\w_]+)\s*,\s*Path\s*\=\s*([\w_]+)\}");

            return true;
        }

        private static Vector3 Vector3Parser(string text)
        {
            string[] splitted = text.Replace("(", "").Replace(")", "").Split(',');
            return new Vector3(float.Parse(splitted[0]), float.Parse(splitted[1]), float.Parse(splitted[2]));
        }

        private static Vector2 Vector2Parser(string text)
        {
            string[] splitted = text.Replace("(", "").Replace(")", "").Split(',');
            return new Vector2(float.Parse(splitted[0]), float.Parse(splitted[1]));
        }
        private static void TextHandler(GameObject gameObject, List<(string, object)> parsedAttributes)
        {
            //TODO: Implement properties for text
            (gameObject.GetComponent<Text>() ?? gameObject.GetComponentInChildren<Text>()).text =
                parsedAttributes[0].Item2 as string;
        }

        private static List<(string, object)> TextParser(XmlAttribute attribute)
        {
            //TODO: Implement properties for text
            return new List<(string, object)> { ("text", attribute.InnerText) };
        }
        private static void TransformHandler(GameObject gameObject, List<(string, object)> parsedObjects)
        {
            foreach ((string name, object value) in parsedObjects)
            {
                switch (name)
                {
                    case "localPosition":
                        gameObject.transform.localPosition = (Vector3)value;
                        break;
                    case "position":
                        gameObject.transform.position = (Vector3)value;
                        break;
                    case "eulerAngles":
                        gameObject.transform.eulerAngles = (Vector3)value;
                        break;
                    case "localEulerAngles":
                        gameObject.transform.localEulerAngles = (Vector3)value;
                        break;
                    case "localScale":
                        gameObject.transform.localScale = (Vector3)value;
                        break;
                    case "localRotation":
                        gameObject.transform.localRotation = (Quaternion)value;
                        break;
                }
            }
        }
        private static void RectTransformHandler(GameObject gameObject, List<(string, object)> parsedObjects)
        {
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            foreach ((string name, object value) in parsedObjects)
            {
                switch (name)
                {
                    case "anchorMin":
                        rectTransform.anchorMin = (Vector2)value;
                        break;
                    case "anchorMax":
                        rectTransform.anchorMax = (Vector2)value;
                        break;
                    case "pivot":
                        rectTransform.pivot = (Vector2)value;
                        break;
                    case "sizeDelta":
                        rectTransform.sizeDelta = (Vector2)value;
                        break;
                    case "anchoredPosition":
                        rectTransform.anchoredPosition = (Vector2)value;
                        break;
                    case "anchoredPosition3D":
                        rectTransform.anchoredPosition3D = (Vector3)value;
                        break;
                }
            }
        }

        private static Sprite SpriteParser(XmlAttribute xmlAttribute)
        {
            return Resources.Load<Sprite>(xmlAttribute.InnerText);
        }

        private static void SpriteHandler(GameObject gameObject, Sprite sprite)
        {
            gameObject.GetComponent<Image>().sprite = sprite;
        }

        private static GameObject PrefabParser(XmlAttribute attribute)
        {
            return Resources.Load<GameObject>(attribute.InnerText);
        }


        private static Dictionary<string, object> ParseAttributes(XmlAttributeCollection attributes)
        {
            Dictionary<string, object> parsedAttributes = new Dictionary<string, object>();
            foreach (XmlAttribute attribute in attributes)
            {
                string name = attribute.Name;
                if (AttributeParser.ContainsKey(name))
                {
                    parsedAttributes.Add(name, AttributeParser[name](attribute));
                }
            }
            return parsedAttributes;
        }
    }
}

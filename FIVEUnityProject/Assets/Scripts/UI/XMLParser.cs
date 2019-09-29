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
        private static readonly Dictionary<string, Func<XmlAttribute, object>> AttributeParser = new Dictionary<string, Func<XmlAttribute, object>>
        {
            {"name", x => x.InnerText },
            {"Prefab", PrefabParser},
            {"NativeSize", a =>
                {
                    bool.TryParse(a.InnerText, out bool result);
                    return result;
                }
            },
            {nameof(Text), TextParser},
            {nameof(Sprite), SpriteParser},
            {nameof(Transform), ComplexAttributeParser},
            {nameof(RectTransform), ComplexAttributeParser},
            {"lineType", EnumParser<InputField.LineType>},
            {"alignment", EnumParser<TextAnchor>},
            {"font", FontParser},
            {"fontSize", FontParser},
        };

        private static object FontParser(XmlAttribute attr)
        {
            return Resources.Load<Font>($"Fonts/{attr.InnerText}");
        }

        private static object NumberParser(XmlAttribute attr)
        {
            return float.Parse(attr.InnerText);
        }

        private static readonly Dictionary<string, Func<string, object>> ComplexAttributeParsers = 
            new Dictionary<string, Func<string, object>>()
        {
            {"anchorMin", Vector2Parser},
            {"anchorMax", Vector2Parser},
            {"pivot", Vector2Parser},
            {"sizeDelta", Vector2Parser},
            {"anchoredPosition", Vector2Parser},
            {"anchoredPosition3D", Vector3Parser},
            {"localPosition", Vector3Parser},
            {"position", Vector3Parser},
            {"offsetMax", Vector2Parser},
            {"offsetMin", Vector2Parser},
        };

        private static object EnumParser<T>(XmlAttribute attribute)
        {
            return Enum.Parse(typeof(T), attribute.InnerText);
        }

        private static object ComplexAttributeParser(XmlAttribute attribute)
        {
            List<(string, object)> parsedObjects = new List<(string, object)>();
            Regex r = new Regex(@"\s*([\w]+)\s*\:\s*([\w\.\,\-\+\(\)]+)\;\s*");
            MatchCollection matchCollection = r.Matches(attribute.InnerText);

            foreach (Match match in matchCollection)
            {
                GroupCollection captures = match.Groups;
                string name = captures[1].Value;
                string value = captures[2].Value;
                object parsedValue = ComplexAttributeParsers[name](value);
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

        private static object Vector3Parser(string text)
        {
            string[] splitted = text.Replace("(", "").Replace(")", "").Split(',');
            return new Vector3(float.Parse(splitted[0]), float.Parse(splitted[1]), float.Parse(splitted[2]));
        }

        private static object Vector2Parser(string text)
        {
            string[] splitted = text.Replace("(", "").Replace(")", "").Split(',');
            return new Vector2(float.Parse(splitted[0]), float.Parse(splitted[1]));
        }


        private static List<(string, object)> TextParser(XmlAttribute attribute)
        {
            //TODO: Implement properties for text
            return new List<(string, object)> { ("text", attribute.InnerText) };
        }


        private static object SpriteParser(XmlAttribute xmlAttribute)
        {
            return Resources.Load<Sprite>(xmlAttribute.InnerText);
        }


        private static object PrefabParser(XmlAttribute attribute)
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

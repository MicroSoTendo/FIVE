using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public partial class XMLDeserializer
    {
        private static readonly Dictionary<string, Action<GameObject, object>> AttributeHandler =
            new Dictionary<string, Action<GameObject, object>>
            {
                {"name", NameHandler},
                {"NativeSize", NativeSizeHandler},
                {nameof(Text), TextHandler},
                {nameof(Transform), TransformHandler},
                {nameof(Sprite), SpriteHandler},
                {nameof(RectTransform), RectTransformHandler},
                {"lineType", LineTypeHandler },
            };

        private static void LineTypeHandler(GameObject go, object value)
        {
            go.GetComponent<InputField>().lineType = (InputField.LineType)value;
        }

        private static void NameHandler(GameObject go, object value)
        {
            go.name = (string)value;
        }        
        
        
        private static void NativeSizeHandler(GameObject go, object value)
        {
            if ((bool)value)
            {
                go.GetComponent<Image>().SetNativeSize();
            }
        }

        private static void TextHandler(GameObject gameObject, object parsedAttributes)
        {
            //TODO: Implement properties for text
            List<(string, object)> listOfAttribtues = (List<(string, object)>)parsedAttributes;
            (gameObject.GetComponent<Text>() ?? gameObject.GetComponentInChildren<Text>()).text =
                listOfAttribtues[0].Item2 as string;
        }




        private static void TransformHandler(GameObject gameObject, object parsedObjects)
        {
            foreach ((string name, object value) in (List<(string, object)>)parsedObjects)
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
        private static void RectTransformHandler(GameObject gameObject, object parsedObjects)
        {
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            foreach ((string name, object value) in (List<(string, object)>)parsedObjects)
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
        private static void SpriteHandler(GameObject gameObject, object sprite)
        {
            gameObject.GetComponent<Image>().sprite = (Sprite)sprite;
        }
    }
}

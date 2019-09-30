using System;
using System.Collections;
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
        public Dictionary<string, GameObject> UIElements { get; }
        public Dictionary<string, GameObject> CanvasResources { get; }
        protected GameObject CanvasGO;
        protected CanvasScaler canvasScaler;
        protected GraphicRaycaster graphicRaycaster;
        protected XMLDeserializer xmlDeserializer;
        protected View()
        {
            CanvasGO = new GameObject { name = GetType().Name };
            ViewCanvas = CanvasGO.AddComponent<Canvas>();
            canvasScaler = CanvasGO.AddComponent<CanvasScaler>();
            graphicRaycaster = CanvasGO.AddComponent<GraphicRaycaster>();
            UIElements = new Dictionary<string, GameObject>();
            CanvasResources = new Dictionary<string, GameObject>();
#if DEBUG
            string xmlText = File.ReadAllText($"{Application.dataPath}/Resources/UI/{GetType().Name}.xml");
#else
            var xmlText = Resources.Load<TextAsset>($"UI/{GetType().Name}").text;
#endif
            xmlDeserializer = new XMLDeserializer(xmlText, ViewCanvas);
            ViewCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            AutoLoad(this);
            LoadResources();
        }

        private static void AutoLoad(View view)
        {
            IEnumerable<PropertyInfo> uiPropInfos = view.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(UIElementAttribute), false)).ToArray();
            foreach (PropertyInfo uiProperty in uiPropInfos)
            {
                Type type = uiProperty.PropertyType;
                string propertyName = uiProperty.Name;
                UIElementAttribute attribute = uiProperty.GetCustomAttribute<UIElementAttribute>();
                switch (attribute.TargetType)
                {
                    case TargetType.Default:
                        uiProperty.SetValue(view, view.AddUIElement(propertyName, type));
                        break;
                    case TargetType.XML:
                        break;
                    case TargetType.Property:
                        PropertyInfo parent = uiPropInfos.First(prop => prop.Name == attribute.Path);
                        GameObject parentGo = (parent.GetValue(view) as MonoBehaviour)?.gameObject ?? parent.GetValue(view) as GameObject;
                        GameObject childRoot = parentGo.GetComponentsInChildren(type, true).First(c => c.name == propertyName).gameObject;
                        uiProperty.SetValue(view, type == typeof(GameObject) ?
                            childRoot : (object)childRoot.GetComponent(type));
                        break;
                    case TargetType.Field:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public object AddUIElement(string name, Type type)
        {
            xmlDeserializer.Deserialize(name, out GameObject gameObject);
            UIElements.Add(name, gameObject);
            return gameObject.GetType() == type ? gameObject : (object)gameObject.GetComponent(type);
        }

        public T AddUIElement<T>(string name)
        {
            xmlDeserializer.Deserialize(name, out GameObject gameObject);
            UIElements.Add(name, gameObject);
            return gameObject is T go ? go : gameObject.GetComponent<T>();
        }

        public T AddUIElementFromResources<T>(string resourceName, string gameObjectName, Transform parent)
        {
            GameObject prefab = CanvasResources[resourceName];
            GameObject gameObject = GameObject.Instantiate(prefab, parent);
            gameObject.SetActive(true);
            gameObject.name = gameObjectName;
            UIElements.Add(gameObjectName, gameObject);
            return gameObject is T go ? go : gameObject.GetComponent<T>();
        }

        public void RemoveUIElement(string name)
        {
            GameObject go = UIElements[name];
            UIElements.Remove(name);
            go.transform.SetParent(null);
            go.SetActive(false);
            GameObject.Destroy(go);
        }

        public T GetUIElement<T>(string name, bool includeInactive = false) where T : MonoBehaviour
        {
            return CanvasGO.GetComponentsInChildren(typeof(T), includeInactive).Cast<T>().FirstOrDefault(child => child.name == name);
        }
#if DEBUG
        public void Unload()
        {
            foreach (GameObject gameObject in UIElements.Values.ToArray())
            {
                gameObject.SetActive(false);
                GameObject.Destroy(gameObject);
            }

            foreach (GameObject gameObject in CanvasResources.Values)
            {
                gameObject.SetActive(false);
                GameObject.Destroy(gameObject);
            }

            UIElements.Clear();
            CanvasResources.Clear();
            GameObject.Destroy(CanvasGO);
        }
#endif
        protected void LoadResources()
        {
            xmlDeserializer.LoadResources(CanvasResources);
        }

        protected static readonly Dictionary<Type, ConstructorInfo> CachedViews = new Dictionary<Type, ConstructorInfo>();
        public static IEnumerator Initialize()
        {
            IEnumerable<Type> types =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where typeof(View).IsAssignableFrom(type)
                select type;
            yield return null;
            foreach (Type t in types)
            {
                ConstructorInfo ctor = t.GetConstructor(Type.EmptyTypes);
                yield return null;
                if (ctor != null && ctor.IsPublic)
                {
                    CachedViews.Add(t, ctor);
                }
                yield return null;
            }
        }
    }

    public abstract class View<TView, TViewModel> : View
        where TView : View<TView, TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {
        public static T Create<T>() where T : View<TView, TViewModel>, new()
        {
            if (CachedViews.ContainsKey(typeof(T)))
            {
                T v = (T)CachedViews[typeof(T)].Invoke(null);
                return v;
            }
            T newView = new T();
            return newView;
        }
    }
}
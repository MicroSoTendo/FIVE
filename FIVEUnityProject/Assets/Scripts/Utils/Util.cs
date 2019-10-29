using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FIVE
{
    public static class Util
    {
        public static void SetParent(this GameObject gameObject, Transform parent)
        {
            gameObject.GetComponent<Transform>().SetParent(parent);
        }

        public static void SetParent(this GameObject gameObject, GameObject parent)
        {
            gameObject.GetComponent<Transform>().SetParent(parent.transform);
        }

        public static GameObject FindChild(this GameObject gameObject, string name)
        {
            return gameObject.transform.Find(name)?.gameObject;
        }

        public static GameObject FindChildRecursive(this GameObject gameObject, string name)
        {
            Transform child = gameObject.transform.Find(name);
            if (child != null)
            {
                return child.gameObject;
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                child = gameObject.transform.GetChild(i);
                Transform grandSon = child.Find(name);
                if (grandSon != null)
                {
                    return grandSon.gameObject;
                }

                GameObject childInChild = child.gameObject.FindChildRecursive(name);
                if (childInChild != null)
                {
                    return childInChild;
                }
            }

            return null;
        }

        public static List<GameObject> GetChildGameObjects(this GameObject gameObject)
        {
            var results = new List<GameObject>();
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                results.Add(child);
                results.AddRange(child.GetChildGameObjects());
            }

            return results;
        }
        public static IEnumerable<Type> GetDerived<T>()
        {
            return Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(t => t.IsSubclassOf(typeof(T)));
        }

        public static IEnumerable<Type> GetDerived(Type t)
        {
            return Assembly.GetAssembly(t).GetTypes().Where(type => type.IsSubclassOf(t));
        }
    }
}
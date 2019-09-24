using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    public static class Util
    {

        public static GameObject GetChildGameObject(this GameObject gameObject, string name)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                GameObject child = gameObject.transform.GetChild(i).gameObject;
                if (child.name == name)
                {
                    return child;
                }
                else
                {
                    GameObject childInChild = child.GetChildGameObject(name);
                    if (childInChild != null)
                    {
                        return childInChild;
                    }
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
    }
}

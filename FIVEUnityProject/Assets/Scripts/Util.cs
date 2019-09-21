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
    }
}

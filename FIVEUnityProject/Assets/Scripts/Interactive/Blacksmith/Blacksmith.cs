using FIVE.Robot;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Interactive.Blacksmith
{
    public class Blacksmith
    {
        public static GameObject GenerateOut(List<GameObject> ItemsIn)
        {
            return ComponentsRead.GenerateItem(ItemsIn);
        }
    }
}
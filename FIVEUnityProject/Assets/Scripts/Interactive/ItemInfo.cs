using System;
using UnityEngine;

namespace FIVE.Interactive
{
    [Serializable]
    public struct ItemInfo
    {
        public string Name;
        [TextArea]
        public string Description;
        public Vector3 UIScale;
        public Vector3 UIRotation;
        public Vector3 UIPosition;
        public static ItemInfo Empty => new ItemInfo { Name = "", Description = "" };
        public static ItemInfo Test => new ItemInfo { Name = "Test Item Type", Description = "Test Item Descriptions. This is a test description, which is only for test purpose and should not be used for non-test purpose. Test description tested is a testing test." };
    }
}

using UnityEngine;
using static FIVE.Network.NetworkUtil;

namespace FIVE.Network
{
    public class TransformSerializer
    {
        public static byte[] Serialize(Transform transform)
        {
            return Combine(transform.eulerAngles.ToBytes(), transform.position.ToBytes());
        }

        public static void Deserialize(byte[] bytes, Transform transform)
        {
            transform.eulerAngles = bytes.ToVector3();
            transform.position = bytes.ToVector3(24);
        }
    }
}

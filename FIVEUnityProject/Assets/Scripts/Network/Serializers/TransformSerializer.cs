using FIVE.Network.Core;
using UnityEngine;

namespace FIVE.Network.Serializers
{
    public static class TransformSerialize
    {
        public static int GetSize(this Serializer<Transform> transform)
        {
            return 25;
        }

        public static void Deserialize(this Serializer<Transform> _, in byte[] bytes, Transform obj)
        {
            Vector3 eulerAngles = bytes.As<Vector3>();
            Vector3 position = bytes.As<Vector3>(12);
            obj.eulerAngles = eulerAngles;
            obj.position = position;
        }

        public static unsafe void Deserialize(this Serializer<Transform> _, byte* bytes, Transform transform)
        {
            transform.rotation = Quaternion.Euler(*(Vector3*)bytes);
            transform.position = *((Vector3*)bytes + 1);
        }

        public static unsafe void Serialize(this Serializer<Transform> _, in Transform transform, in byte[] bytes, int startIndex = 0)
        {
            fixed (byte* p = bytes)
            {
                byte* pBytes = p + startIndex;
                bytes[startIndex] = (byte)ComponentType.Transform;
                *(Vector3*)(pBytes + 1) = transform.rotation.eulerAngles;
                *(Vector3*)(pBytes + 13) = transform.position;
            }
        }
    }
}

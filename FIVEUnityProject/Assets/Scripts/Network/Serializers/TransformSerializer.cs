using UnityEngine;
namespace FIVE.Network.Serializers
{
    public static class TransformSerialize
    {
        public static int GetSize(this Serializer<Transform> transform) => 24;

        public static void Deserialize(this Serializer<Transform> transform, in byte[] bytes, Transform obj)
        {
            Vector3 eulerAngles = bytes.ToVector3();
            Vector3 position = bytes.ToVector3(12);
            obj.eulerAngles = eulerAngles;
            obj.position = position;
        }

        public static void Serialize(this Serializer<Transform> transform, in Transform obj, in byte[] bytes, int startIndex = 0)
        {
            bytes.CopyFromUnsafe(obj.eulerAngles.ToBytes(), obj.position.ToBytes(), startIndex);
        }
    }
}

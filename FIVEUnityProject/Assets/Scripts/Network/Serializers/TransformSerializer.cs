using UnityEngine;
namespace FIVE.Network.Serializers
{
    public class TransformSerializer : Serializer<Transform>
    {
        public override void Deserialize(in byte[] bytes, Transform obj)
        {
            Vector3 eulerAngles = bytes.ToVector3();
            Vector3 position = bytes.ToVector3(12);
            obj.eulerAngles = eulerAngles;
            obj.position = position;
        }

        public override void Serialize(in Transform obj, out byte[] bytes)
        {
            bytes = NetworkUtil.Combine(obj.eulerAngles.ToBytes(), obj.position.ToBytes());
        }
    }
}

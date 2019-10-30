using UnityEngine;
namespace FIVE.Network.Serializers
{
    public sealed class TransformSerializer : Serializer<Transform>
    {

        static TransformSerializer()
        {
            Instance = new TransformSerializer();
            SerializedSizes.Add(typeof(Transform), 24);
        }

        private TransformSerializer() { }

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

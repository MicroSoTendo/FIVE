using System.Collections.Generic;
using UnityEngine;
namespace FIVE.Network.Serializers
{
    public abstract class Serializer
    {
        public static void Serialize(List<(int componentID, Component component)> components, in byte[] destination, int destStartIndex = 0)
        {
            int offset = destStartIndex;
            foreach ((int id, Component component) in components)
            {
                ListSerializeHelper(id, component, destination, ref offset);
            }
        }


        public static int GetSize(Component component)
        {
            int result = 0;
            switch (component)
            {
                case Transform _:
                    result = Serializer<Transform>.Instance.GetSize();
                    break;
                case Animator _:
                    break;
                default:
                    break;
            }

            return result;
        }

        private static void ListSerializeHelper<T>(int id, T obj, byte[] bytes, ref int offset)
        {
            switch (obj)
            {
                case Transform transform:
                    Serializer<Transform>.Instance.Serialize(transform, bytes, offset);
                    offset += Serializer<Transform>.Instance.GetSize();
                    break;
                case Animator animator:
                    break;
            }
        }
    }

    public sealed class Serializer<T> : Serializer
    {
        public static Serializer<T> Instance { get; } = new Serializer<T>();
        private Serializer() { }
    }
}

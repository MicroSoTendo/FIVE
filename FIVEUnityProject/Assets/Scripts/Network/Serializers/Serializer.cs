using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        public static void Deserialize(GameObject go, byte[] buffer, int startIndex = 0)
        {
            
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

        public static bool TrySerialize(int id, Component c, out byte[] bytes)
        {
            switch (c)
            {
                case Animator animator:
                    break;
                case Transform transform:
                    if (transform.hasChanged)
                    {
                        bytes = new byte[28];
                        id.CopyTo(bytes);
                        Serializer<Transform>.Instance.Serialize(transform, bytes, 4);
                        return true;
                    }
                    break;
                default:
                    break;
            }
            bytes = default;
            return false;
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

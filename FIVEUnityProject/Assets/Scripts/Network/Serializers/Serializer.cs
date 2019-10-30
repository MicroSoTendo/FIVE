using System;
using System.Collections.Generic;
using UnityEngine;
using static FIVE.Util;
namespace FIVE.Network.Serializers
{
    public abstract class Serializer 
    {
        //private static readonly Dictionary<Type, Serializer> Serializers = new Dictionary<Type, Serializer>();
        //static Serializer()
        //{
        //    foreach (Type type in GetDerived(typeof(Serializer)))
        //    {
        //        Type componentType = type.BaseType.GenericTypeArguments[0];
        //        Serializers.Add(componentType, type.GetConstructor(Type.EmptyTypes).Invoke(null) as Serializer);
        //    }
        //}

        //public static Serializer<T> Get<T>()
        //{
        //    return (Serializer<T>)Serializers[typeof(T)];
        //}

        protected static readonly Dictionary<Type, int> SerializedSizes = new Dictionary<Type, int>();
        protected static readonly Dictionary<Type, ComponentType> EnumMap = new Dictionary<Type, ComponentType>();
        public static int GetSize(Type t)
        {
            return SerializedSizes[t];
        }

        public static void Serialize(List<Component> components, byte[] bytes)
        {
            int offset = 0;
            foreach (Component component in components)
            {
                switch (component)
                {
                    case Transform transform:
                        DoSerialize(transform, bytes, ref offset);
                        break;
                    case Animator animator:
                        DoSerialize(animator, bytes, ref offset);
                        break;
                        //TODO: Finish other cases
                }
            }
        }

        private static void DoSerialize<T>(T obj, byte[] bytes, ref int offset)
        {
            bytes.CopyFromUnsafe(EnumMap[typeof(T)].ToBytes(), offset);
            offset += 4;
            Serializer<T>.Instance.Serialize(obj, out byte[] buffer);
            bytes.CopyFromUnsafe(buffer, offset);
            offset += GetSize(typeof(T));
        }
    }

    public abstract class Serializer<T> : Serializer
    {
        public static Serializer<T> Instance { get; protected set; }
        public abstract void Serialize(in T obj, out byte[] bytes);
        public abstract void Deserialize(in byte[] bytes, T obj);
        
    }
}
